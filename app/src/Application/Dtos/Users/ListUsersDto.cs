namespace Application.Dtos.Users;

public class ListUsersDto
{
	public int? Page { get; set; }
	public int? PageSize { get; set; }
	public string? Search { get; set; }
}
