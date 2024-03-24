using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestsUnitairesPourServices.Models;

namespace TestsUnitairesPourServices.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext (DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<Cat> Cat { get; set; } = default!;
        public DbSet<House> House { get; set; } = default!;
    }
}
