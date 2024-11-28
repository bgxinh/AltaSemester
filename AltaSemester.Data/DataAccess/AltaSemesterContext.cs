using AltaSemester.Data.Entities;
using AltaSemester.Data.Enums;
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
        public DbSet<Entities.Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceService> DevicesServices { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ServiceTicket> ServiceTickets { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Định nghĩa quan hệ giữa User và Role
            modelBuilder.Entity<User>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<Entities.Role>()
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
            //Định nghĩa quan hệ giữa Device và Service
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
            //Định nghĩa quan hệ giữa Role và Permission
            modelBuilder.Entity<Permission>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<RolePermission>()
                .HasKey(x => new { x.RoleId, x.PermissionId });
            modelBuilder.Entity<RolePermission>()
                .HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RolePermission>()
                .HasOne(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            //Định nghĩa quan hệ giữa user, device, service và serviceTicket
            modelBuilder.Entity<ServiceTicket>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<ServiceTicket>()
                .HasOne(x => x.User)
                .WithMany(x => x.ServiceTickets)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ServiceTicket>()
                .HasOne(x => x.Service)
                .WithMany(x => x.ServiceTickets)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ServiceTicket>()
                .HasOne(x => x.Device)
                .WithMany(x => x.ServiceTickets)
                .HasForeignKey(x => x.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
            //Bảng actitvityLog
            modelBuilder.Entity<ActivityLog>()
                .HasKey(x => x.Id);
        }
    }
}
