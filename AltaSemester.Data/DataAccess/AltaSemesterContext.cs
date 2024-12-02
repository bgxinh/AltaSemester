using AltaSemester.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.DataAccess
{
    public class AltaSemesterContext : DbContext
    {
        public AltaSemesterContext(DbContextOptions<AltaSemesterContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<Device>()
                .HasKey(x => x.DeviceCode);
            modelBuilder.Entity<Service>()
                .HasKey(x => x.ServiceCode);
            modelBuilder.Entity<Assignment>()
                .HasKey(x => new {x.Code, x.AssignmentDate});
            modelBuilder.Entity<Assignment>()
                .HasOne(x => x.Device)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.DeviceCode)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Assignment>()
                .HasOne(x => x.Service)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.ServiceCode)
                .OnDelete(DeleteBehavior.Cascade);
            //Bảng actitvityLog
            modelBuilder.Entity<ActivityLog>()
                .HasKey(x => x.Id);
        }
    }
}
