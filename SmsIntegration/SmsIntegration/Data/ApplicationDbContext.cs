using Microsoft.EntityFrameworkCore;
using SmsIntegration.Models;



namespace SmsIntegration.Data

{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        public DbSet<User> Users { get; set; }  
        public DbSet<TwilioSettings> TwilioSettings { get; set; }
    }
}
