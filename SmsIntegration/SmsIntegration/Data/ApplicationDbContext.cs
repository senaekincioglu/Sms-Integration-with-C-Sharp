using Microsoft.EntityFrameworkCore;


namespace SmsIntegration.Data

{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
    }
}
