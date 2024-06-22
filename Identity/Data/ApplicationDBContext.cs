namespace Identity.Data;

public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> Options) : IdentityDbContext<Employee>(Options)
{
    public DbSet<ProfilePicture> ProfilePics { get; set; }

    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
