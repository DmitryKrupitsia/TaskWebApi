using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskWebApi.Models;

namespace TaskWebApi.Data;

public partial class HysTestTaskDbContext : DbContext
{
    public HysTestTaskDbContext()
    {
    }

    public HysTestTaskDbContext(DbContextOptions<HysTestTaskDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<DslProduct> DslProducts { get; set; }

    public virtual DbSet<TvProduct> TvProducts { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        var dateOnlyConverter = new ValueConverter<DateOnly?, DateTime?>(
            d => d.HasValue
                   ? d.Value.ToDateTime(TimeOnly.MinValue)
                   : (DateTime?)null,
            dt => dt.HasValue
                   ? DateOnly.FromDateTime(dt.Value)
                   : (DateOnly?)null
        );

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07F531BA0E");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<DslProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DslProdu__3214EC07820E2849");

            entity.Property(e => e.Product).HasMaxLength(50);
            entity.Property(e => e.StartDate)
                .HasConversion(dateOnlyConverter)
                .HasColumnType("date");
            entity.Property(e => e.EndDate)
                .HasConversion(dateOnlyConverter)
                .HasColumnType("date");
        });

        modelBuilder.Entity<TvProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TvProduc__3214EC07683A3967");

            entity.Property(e => e.Product).HasMaxLength(50);
            entity.Property(e => e.StartDate)
                .HasConversion(dateOnlyConverter)
                .HasColumnType("date");

            entity.Property(e => e.EndDate)
                .HasConversion(dateOnlyConverter)
                .HasColumnType("date");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
