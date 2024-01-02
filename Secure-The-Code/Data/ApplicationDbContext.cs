using Microsoft.EntityFrameworkCore;
using Secure_The_Code.Models;

namespace Secure_The_Code.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) { }


        public DbSet<Customer> Customers { get; set; }


    }
}
