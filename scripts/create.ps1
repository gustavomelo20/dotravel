param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Name
)

function Get-Singular([string]$plural) {
    if ($plural -match '(?i)ies$') { return $plural -replace '(?i)ies$', 'y' }
    if ($plural -match '(?i)s$') { return $plural -replace '(?i)s$', '' }
    return $plural
}

function PascalCase([string]$text) {
    $parts = $text -split '[^a-zA-Z0-9]+'
    ($parts | Where-Object { $_ -ne '' } | ForEach-Object { $_.Substring(0,1).ToUpper() + $_.Substring(1).ToLower() }) -join ''
}

function Ensure-Directory([string]$path) {
    if (-not (Test-Path $path)) { New-Item -ItemType Directory -Path $path | Out-Null }
}

function Write-File([string]$path, [string]$content) {
    if (-not (Test-Path $path)) {
        $dir = Split-Path $path -Parent
        Ensure-Directory $dir
        Set-Content -Path $path -Value $content -Encoding UTF8
        Write-Host "Created: $path"
    } else {
        Write-Host "Skipped (exists): $path"
    }
}

function Add-UsingIfMissing([string]$file, [string]$usingNs) {
    $content = Get-Content -Path $file -Raw
    if ($content -notmatch [regex]::Escape("using $usingNs;")) {
        $updated = "using $usingNs;`r`n" + $content
        Set-Content -Path $file -Value $updated -Encoding UTF8
        Write-Host "Updated using in: $file"
    }
}

function Add-ScopedIfMissing([string]$file, [string]$registration) {
    $content = Get-Content -Path $file -Raw
    if ($content -notmatch [regex]::Escape($registration)) {
        $pattern = 'private static void AddRepositories\(IServiceCollection services\)\s*\{'
        if ($content -match $pattern) {
            $updated = [System.Text.RegularExpressions.Regex]::Replace($content, $pattern, {
                param($m)
                return $m.Value + "`r`n        $registration"
            }, 1)
            Set-Content -Path $file -Value $updated -Encoding UTF8
            Write-Host "Registered in DI: $registration"
        } else {
            Write-Warning "Could not locate AddRepositories method in $file"
        }
    }
}

function Add-DbSetIfMissing([string]$file, [string]$entitySingular, [string]$entityPlural, [string]$entitiesNamespace) {
    $content = Get-Content -Path $file -Raw
    $usingNs = "using $entitiesNamespace;"
    if ($content -notmatch [regex]::Escape($usingNs)) {
        $content = $usingNs + "`r`n" + $content
    }
    $dbsetLine = "    public DbSet<$entitySingular> $entityPlural { get; set; }"
    if ($content -notmatch [regex]::Escape($dbsetLine)) {
        # Insert right after the opening brace of DataBseContext class (compatible with Windows PowerShell regex)
        $pattern = '(class\s+DataBseContext\s*:\s*DbContext\s*\{)'
        if ($content -match $pattern) {
            $content = [System.Text.RegularExpressions.Regex]::Replace($content, $pattern, {
                param($m)
                return $m.Groups[1].Value + "`r`n" + $dbsetLine + "`r`n"
            }, 1)
        } else {
            Write-Warning "Could not find DataBseContext class signature in $file"
        }
        Set-Content -Path $file -Value $content -Encoding UTF8
        Write-Host "Added DbSet: $entityPlural in DataBseContext"
    }
}

$root = (Get-Location).Path
$appSrc = Join-Path $root 'app\src'

$pluralRaw = $Name
$plural = PascalCase $pluralRaw
$singular = PascalCase (Get-Singular $pluralRaw)

# Paths
$domain = Join-Path $appSrc 'Domain'
$application = Join-Path $appSrc 'Application'
$infrastructure = Join-Path $appSrc 'Infrastructure'
$webapi = Join-Path $appSrc 'WebApi'

$entitiesNs = "Domain.Entities.$plural"
$reposNs = "Domain.Repositories.$plural"
$infraReposNs = "Infrastructure.DataAccess.Repositories.$plural"
$dtosNs = "Application.Dtos.$plural"
$interfacesNs = "Application.Interfaces.$plural"

# Domain: Entity
$entityDir = Join-Path $domain ("Entities\$plural")
Ensure-Directory $entityDir
$entityFile = Join-Path $entityDir ("$singular.cs")
$entityContent = @"
namespace $entitiesNs;

public class $singular
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
"@
Write-File $entityFile $entityContent

# Domain: Repositories interfaces
$repoDir = Join-Path $domain ("Repositories\$plural")
Ensure-Directory $repoDir
$iroFile = Join-Path $repoDir ("I${singular}ReadOnlyRepository.cs")
$iwoFile = Join-Path $repoDir ("I${singular}WriteOnlyRepository.cs")
$iroContent = @"
using $entitiesNs;

namespace $reposNs;

public interface I${singular}ReadOnlyRepository
{
    Task<$singular?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IList<$singular>> ListAsync(string? search = null, int? page = null, int? pageSize = null, CancellationToken cancellationToken = default);
}
"@
$iwoContent = @"
using $entitiesNs;

namespace $reposNs;

public interface I${singular}WriteOnlyRepository
{
    Task<$singular> AddAsync($singular item, CancellationToken cancellationToken = default);
    Task<$singular> UpdateAsync($singular item, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
"@
Write-File $iroFile $iroContent
Write-File $iwoFile $iwoContent

# Infrastructure: Repository
$infraRepoDir = Join-Path $infrastructure ("DataAccess\Repositories\$plural")
Ensure-Directory $infraRepoDir
$infraRepoFile = Join-Path $infraRepoDir ("${singular}Repository.cs")
$infraRepoContent = @"
using $entitiesNs;
using $reposNs;
using Microsoft.EntityFrameworkCore;

namespace $infraReposNs;

public class ${singular}Repository : I${singular}ReadOnlyRepository, I${singular}WriteOnlyRepository
{
    private readonly DataBseContext _context;

    public ${singular}Repository(DataBseContext context)
    {
        _context = context;
    }

    public async Task<$singular?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.$plural
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IList<$singular>> ListAsync(string? search = null, int? page = null, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        IQueryable<$singular> query = _context.$plural.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(e => EF.Functions.Like(e.Name, $"%{term}%"));
        }

        if (page.HasValue && pageSize.HasValue && page.Value > 0 && pageSize.Value > 0)
        {
            var skip = (page.Value - 1) * pageSize.Value;
            query = query.Skip(skip).Take(pageSize.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<$singular> AddAsync($singular item, CancellationToken cancellationToken = default)
    {
        await _context.$plural.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<$singular> UpdateAsync($singular item, CancellationToken cancellationToken = default)
    {
        var existing = await _context.$plural.FirstOrDefaultAsync(e => e.Id == item.Id, cancellationToken);
        if (existing is null)
        {
            throw new KeyNotFoundException($"$singular with id {item.Id} not found");
        }

        existing.Name = item.Name;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.$plural.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        _context.$plural.Remove(existing);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
"@
Write-File $infraRepoFile $infraRepoContent

# Application: DTOs
$dtosDir = Join-Path $application ("Dtos\$plural")
Ensure-Directory $dtosDir
Write-File (Join-Path $dtosDir ("Create$singular`Dto.cs")) (@"
namespace $dtosNs;

public class Create${singular}Dto
{
    public string Name { get; set; } = string.Empty;
}
"@)
Write-File (Join-Path $dtosDir ("Delete$singular`Dto.cs")) (@"
namespace $dtosNs;

public class Delete${singular}Dto
{
    public Guid Id { get; set; }
}
"@)
Write-File (Join-Path $dtosDir ("Get$singular`ByIdDto.cs")) (@"
namespace $dtosNs;

public class Get${singular}ByIdDto
{
    public Guid Id { get; set; }
}
"@)
Write-File (Join-Path $dtosDir ("List$plural`Dto.cs")) (@"
namespace $dtosNs;

public class List${plural}Dto
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? Search { get; set; }
}
"@)
Write-File (Join-Path $dtosDir ("Update$singular`Dto.cs")) (@"
namespace $dtosNs;

public class Update${singular}Dto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}
"@)

# Application: Interfaces
$appInterfacesDir = Join-Path $application ("Interfaces\$plural")
Ensure-Directory $appInterfacesDir
Write-File (Join-Path $appInterfacesDir ("ICreate$singular.cs")) (@"
using $dtosNs;
using $entitiesNs;

namespace $interfacesNs;

public interface ICreate$singular
{
    Task<$singular> ExecuteAsync(Create${singular}Dto input, CancellationToken cancellationToken = default);
}
"@)
Write-File (Join-Path $appInterfacesDir ("IDelete$singular.cs")) (@"
using $dtosNs;

namespace $interfacesNs;

public interface IDelete$singular
{
    Task<bool> ExecuteAsync(Delete${singular}Dto input, CancellationToken cancellationToken = default);
}
"@)
Write-File (Join-Path $appInterfacesDir ("IGet$singular`ById.cs")) (@"
using $dtosNs;
using $entitiesNs;

namespace $interfacesNs;

public interface IGet${singular}ById
{
    Task<$singular?> ExecuteAsync(Get${singular}ByIdDto input, CancellationToken cancellationToken = default);
}
"@)
Write-File (Join-Path $appInterfacesDir ("IList$plural.cs")) (@"
using $dtosNs;
using $entitiesNs;

namespace $interfacesNs;

public interface IList$plural
{
    Task<IList<$singular>> ExecuteAsync(List${plural}Dto input, CancellationToken cancellationToken = default);
}
"@)
Write-File (Join-Path $appInterfacesDir ("IUpdate$singular.cs")) (@"
using $dtosNs;
using $entitiesNs;

namespace $interfacesNs;

public interface IUpdate$singular
{
    Task<$singular> ExecuteAsync(Update${singular}Dto input, CancellationToken cancellationToken = default);
}
"@)

# Application: useCases folder structure (empty for now)
$useCasesBase = Join-Path $application ("useCases\$plural")
Ensure-Directory (Join-Path $useCasesBase ("Create$singular"))
Ensure-Directory (Join-Path $useCasesBase ("Delete$singular"))
Ensure-Directory (Join-Path $useCasesBase ("Get${singular}ById"))
Ensure-Directory (Join-Path $useCasesBase ("List$plural"))
Ensure-Directory (Join-Path $useCasesBase ("Update$singular"))

# WebApi: Controller
$controllersDir = Join-Path $webapi 'Controllers'
Ensure-Directory $controllersDir
$controllerFile = Join-Path $controllersDir ("${singular}Controller.cs")
$controllerContent = @"
using $dtosNs;
using $interfacesNs;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class ${singular}Controller : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromServices] ICreate$singular useCase,
        [FromBody] Create${singular}Dto request
    )
    {
        var result = await useCase.ExecuteAsync(request);
        return Ok(result);
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById(
        [FromServices] IGet${singular}ById useCase,
        [FromQuery] Get${singular}ByIdDto request
    )
    {
        var result = await useCase.ExecuteAsync(request);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(
        [FromServices] IList$plural useCase,
        [FromQuery] List${plural}Dto request
    )
    {
        var result = await useCase.ExecuteAsync(request);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(
        [FromServices] IUpdate$singular useCase,
        [FromBody] Update${singular}Dto request
    )
    {
        var result = await useCase.ExecuteAsync(request);
        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(
        [FromServices] IDelete$singular useCase,
        [FromBody] Delete${singular}Dto request
    )
    {
        var success = await useCase.ExecuteAsync(request);
        return Ok(success);
    }
}
"@
Write-File $controllerFile $controllerContent

# Update DbContext and DI
$dbContextFile = Join-Path $infrastructure 'DataBseContext.cs'
Add-DbSetIfMissing -file $dbContextFile -entitySingular $singular -entityPlural $plural -entitiesNamespace $entitiesNs

$diFile = Join-Path $infrastructure 'DependencyInjection.cs'
Add-UsingIfMissing -file $diFile -usingNs $reposNs
Add-UsingIfMissing -file $diFile -usingNs $infraReposNs
Add-ScopedIfMissing -file $diFile -registration "services.AddScoped<I${singular}ReadOnlyRepository, ${singular}Repository>();"
Add-ScopedIfMissing -file $diFile -registration "services.AddScoped<I${singular}WriteOnlyRepository, ${singular}Repository>();"

Write-Host "Scaffold completed for entity: $singular ($plural)"
