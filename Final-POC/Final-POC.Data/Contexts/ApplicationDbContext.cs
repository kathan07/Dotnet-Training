using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Final_POC.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_POC.Data.Contexts
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<TaskDetail> TaskDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // AssignedTo relationship (OnDelete: SetNull)
            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.AssignedTo)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            // CreatedBy relationship (OnDelete: SetNull)
            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.CreatedBy)
                .WithMany() // You can define a navigation collection if needed
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            // Task to TaskDetail relationship (one-to-one)
            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.TaskDetail)
                .WithOne(td => td.Task)
                .HasForeignKey<TaskDetail>(td => td.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
