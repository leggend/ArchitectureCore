using Microsoft.EntityFrameworkCore;
using SplunkFormsManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplunkFormsManager.Infrastrucutre.Context
{
    public class DataContext : DbContext
    {
        public DbSet<DummyEntity> Dummies { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
