namespace Identity.Data;

public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> Options) : IdentityDbContext<Employee>(Options)
{
    public DbSet<ProfilePicture> ProfilePics { get; set; }

    public DbSet<Address> Addresses { get; set; }

    public DbSet<Experience> WorkHistory { get; set; }

    public DbSet<EmployeeTask> EmployeeTasks { get; set; }

    public DbSet<Team> Teams { get; set; }

    public DbSet<TaskDocument> TaskDocuments { get; set; }

    public DbSet<TaskNote> TaskNotes { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);              
    }
}
