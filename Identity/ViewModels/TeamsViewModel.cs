namespace Identity.ViewModels;

public class TeamsViewModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }  

    [Required]
    public string Manager { get; set; }

    public string? Member {  get; set; }

    public List<Employee>? Members { get; set; }

}
