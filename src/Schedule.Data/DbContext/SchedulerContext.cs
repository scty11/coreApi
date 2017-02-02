using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Schedule.Model;
using Schedule.Model.Enums;

namespace Schedule.Data.DbContext
{
    public class SchedulerContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Model.Schedule> Schedules { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Attendee> Attendees { get; set; }

        public SchedulerContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
#region Schedule

            modelBuilder.Entity<Model.Schedule>()
            .ToTable("Schedule");

            modelBuilder.Entity<Model.Schedule>()
                .Property(s => s.CreatorId)
                .IsRequired();

            modelBuilder.Entity<Model.Schedule>()
                .Property(s => s.DateCreated)
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<Model.Schedule>()
                .Property(s => s.DateUpdated)
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<Model.Schedule>()
                .Property(s => s.Type)
                .HasDefaultValue(ScheduleType.Work);

            modelBuilder.Entity<Model.Schedule>()
                .Property(s => s.Status)
                .HasDefaultValue(ScheduleStatus.Valid);

            modelBuilder.Entity<Model.Schedule>()
                .HasOne(s => s.Creator)
                .WithMany(c => c.SchedulesCreated);
#endregion
#region User
            modelBuilder.Entity<User>()
                .ToTable("User");

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .HasMaxLength(100)
                .IsRequired();
#endregion

#region Attendee
            modelBuilder.Entity<Attendee>()
                .ToTable("Attendee");

            modelBuilder.Entity<Attendee>()
                .HasOne(a => a.User)
                .WithMany(u => u.SchedulesAttended)
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<Attendee>()
                .HasOne(a => a.Schedule)
                .WithMany(s => s.Attendees)
                .HasForeignKey(a => a.ScheduleId);
#endregion
        }
    }
}
