﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Mjosc.SimpleLMS.Entities.Models
{
    public partial class LmsDbContext : DbContext
    {
        public LmsDbContext()
        {
        }

        public LmsDbContext(DbContextOptions<LmsDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<Enrollment> Enrollment { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasIndex(e => e.TeacherId)
                    .HasName("TeacherFK_idx");

                entity.Property(e => e.CourseId).HasColumnType("bigint(20)");

                entity.Property(e => e.CourseName)
                    .IsRequired()
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.CreditHours).HasColumnType("smallint(1)");

                entity.Property(e => e.TeacherId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.TeacherId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TeacherFK");
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.CourseId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.CourseId)
                    .HasName("CourseFK_idx");

                entity.Property(e => e.StudentId).HasColumnType("bigint(20)");

                entity.Property(e => e.CourseId).HasColumnType("bigint(20)");

                entity.Property(e => e.Grade).HasColumnType("varchar(2)");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Enrollment)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CourseFK");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Enrollment)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("StudentFK");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnType("bigint(20)");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnType("varchar(12)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnType("varchar(12)");
            });
        }
    }
}
