using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SweazyHut.Data.Context
{
    public class SweazyHutDbContext : DbContext, ISweazyHutDbContext
    {
        private IConfiguration _configuration;

        public SweazyHutDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public SweazyHutDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("ProductionDb"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

         //   modelBuilder.ApplyConfiguration(new CityDictEntityConfiguration());
        }
    }

}

