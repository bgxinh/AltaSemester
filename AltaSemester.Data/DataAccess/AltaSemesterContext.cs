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
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceService> DevicesService { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<Role>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<UserRole>()
                .HasKey(x => new {x.UserId, x.RoleId});
            modelBuilder.Entity<UserRole>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Device>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<Service>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<DeviceService>()
                .HasKey(x => new {x.ServiceId, x.DeviceId});
            modelBuilder.Entity<DeviceService>()
                .HasOne(x => x.Device)
                .WithMany(x => x.DeviceServices)
                .HasForeignKey(x => x.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DeviceService>()
                .HasOne(x => x.Service)
                .WithMany(x => x.DeviceServices)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
