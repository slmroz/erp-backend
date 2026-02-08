using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ERP.Model.Model;

public partial class ErpContext : DbContext
{
    public ErpContext()
    {
    }

    public ErpContext(DbContextOptions<ErpContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("slmroz_dbadmin");

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC073F3A12EF");

            entity.ToTable("Customer", "Crm");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Facebook).HasMaxLength(100);
            entity.Property(e => e.LastModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.RemovedAt).HasColumnType("datetime");
            entity.Property(e => e.TaxId).HasMaxLength(100);
            entity.Property(e => e.Www).HasMaxLength(100);
            entity.Property(e => e.ZipCode).HasMaxLength(10);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07F1B366FD");

            entity.ToTable("User", "Auth");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(200);
            entity.Property(e => e.Role).HasDefaultValue(1);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
