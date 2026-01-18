using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Data.Models;

public partial class ActivityClubDbContext : DbContext
{
    public ActivityClubDbContext()
    {
    }

    public ActivityClubDbContext(DbContextOptions<ActivityClubDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventGuide> EventGuides { get; set; }

    public virtual DbSet<EventMember> EventMembers { get; set; }

    public virtual DbSet<Guide> Guides { get; set; }

    public virtual DbSet<Lookup> Lookups { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Intentionally left blank.
        // DbContext is configured via Dependency Injection in ActivityClub.API (Program.cs)
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Event__7944C810F343F319");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Category).WithMany(p => p.EventCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Category");

            entity.HasOne(d => d.Status).WithMany(p => p.EventStatuses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Status");
        });

        modelBuilder.Entity<EventGuide>(entity =>
        {
            entity.HasKey(e => e.EventGuideId).HasName("PK__EventGui__6BD595F0354C4653");

            entity.Property(e => e.AssignedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Event).WithMany(p => p.EventGuides)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventGuide_Event");

            entity.HasOne(d => d.Guide).WithMany(p => p.EventGuides)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventGuide_Guide");
        });

        modelBuilder.Entity<EventMember>(entity =>
        {
            entity.HasKey(e => e.EventMemberId).HasName("PK__EventMem__0C810311BF68B9F1");

            entity.Property(e => e.JoinDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Event).WithMany(p => p.EventMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventMember_Event");

            entity.HasOne(d => d.Member).WithMany(p => p.EventMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventMember_Member");
        });

        modelBuilder.Entity<Guide>(entity =>
        {
            entity.HasKey(e => e.GuideId).HasName("PK__Guide__E77EE05E0332F846");

            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Guide_IsActive");

            entity.HasOne(d => d.Profession).WithMany(p => p.Guides).HasConstraintName("FK_Guide_Profession");

            entity.HasOne(d => d.User).WithOne(p => p.Guide)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Guide_User");
        });

        modelBuilder.Entity<Lookup>(entity =>
        {
            entity.HasKey(e => e.LookupId).HasName("PK__Lookup__6D8B9C6B0D2F337D");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Member__0CF04B183ECE0956");

            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Member_IsActive");

            entity.HasOne(d => d.Nationality).WithMany(p => p.MemberNationalities).HasConstraintName("FK_Member_Nationality");

            entity.HasOne(d => d.Profession).WithMany(p => p.MemberProfessions).HasConstraintName("FK_Member_Profession");

            entity.HasOne(d => d.User).WithOne(p => p.Member).HasConstraintName("FK_Member_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__ROLES__8AFACE3A57D2DD55");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CB8A3AB44");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.GenderLookup).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Gender");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRole__RoleID__440B1D61"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRole__UserID__4316F928"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF27604FAADC4C3D");
                        j.ToTable("UserRole");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
