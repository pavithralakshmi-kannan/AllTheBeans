using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AllTheBeans.Data;

#nullable disable

namespace AllTheBeans.Migrations
{
    [DbContext(typeof(BeansDb))]
    partial class BeansDbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("AllTheBeans.Models.Bean", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<string>("ExternalId").IsRequired().HasMaxLength(64);
                b.Property<int>("Index");
                b.Property<bool>("IsBOTD");
                b.Property<string>("Name").IsRequired().HasMaxLength(100);
                b.Property<string>("Colour").IsRequired().HasMaxLength(50);
                b.Property<string>("Country").IsRequired().HasMaxLength(50);
                b.Property<string>("Description").IsRequired();
                b.Property<string>("Image").IsRequired().HasMaxLength(500);
                b.Property<decimal>("Cost").HasColumnType("decimal(10,2)");
                b.HasKey("Id");
                b.HasIndex("Name");
                b.ToTable("Beans");
            });

            modelBuilder.Entity("AllTheBeans.Models.Order", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<string>("CustomerEmail").IsRequired().HasMaxLength(256);
                b.Property<DateTime>("CreatedUtc");
                b.Property<int>("BeanId");
                b.Property<int>("Quantity");
                b.Property<decimal>("UnitPrice").HasColumnType("decimal(10,2)");
                b.HasKey("Id");
                b.HasIndex("BeanId");
                b.ToTable("Orders");
            });

            modelBuilder.Entity("AllTheBeans.Models.BotdAssignment", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<DateTime>("Date");
                b.Property<int>("BeanId");
                b.HasKey("Id");
                b.HasIndex("Date").IsUnique();
                b.HasIndex("BeanId");
                b.ToTable("BotdAssignments");
            });

            modelBuilder.Entity("AllTheBeans.Models.Order", b =>
            {
                b.HasOne("AllTheBeans.Models.Bean", "Bean")
                 .WithMany()
                 .HasForeignKey("BeanId")
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("AllTheBeans.Models.BotdAssignment", b =>
            {
                b.HasOne("AllTheBeans.Models.Bean", "Bean")
                 .WithMany()
                 .HasForeignKey("BeanId")
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
