﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SystemInfoApi.Data;

#nullable disable

namespace SystemInfoApi.Migrations
{
    [DbContext(typeof(SystemInfoContext))]
    partial class SystemInfoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SystemInfoApi.Models.CustomerModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_client");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Client");
                });

            modelBuilder.Entity("SystemInfoApi.Models.DriveModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_client_machine_disque");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Format")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("FreeSpace")
                        .HasColumnType("bigint")
                        .HasColumnName("Free_Space");

                    b.Property<int>("FreeSpacePercentage")
                        .HasColumnType("int")
                        .HasColumnName("Free_Space_Percentage");

                    b.Property<int>("IsSystemDrive")
                        .HasColumnType("int")
                        .HasColumnName("Is_System_Drive");

                    b.Property<string>("Label")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MachineId")
                        .HasColumnType("int")
                        .HasColumnName("id_client_machine");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<long>("TotalSpace")
                        .HasColumnType("bigint")
                        .HasColumnName("Total_Space");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MachineId");

                    b.ToTable("Client_Machine_Disque");
                });

            modelBuilder.Entity("SystemInfoApi.Models.MachineModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_client_machine");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CustomerId")
                        .HasColumnType("int")
                        .HasColumnName("id_client");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Client_Machine");
                });

            modelBuilder.Entity("SystemInfoApi.Models.OsModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_client_machine_disque_os");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Architecture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentBuild")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Current_Build");

                    b.Property<string>("Directory")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DriveId")
                        .HasColumnType("int")
                        .HasColumnName("id_client_machine_disque");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Product_Name");

                    b.Property<string>("ReleaseId")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Release_Id");

                    b.Property<string>("Ubr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DriveId")
                        .IsUnique();

                    b.ToTable("Client_Machine_Disque_Os");
                });

            modelBuilder.Entity("SystemInfoApi.Models.DriveModel", b =>
                {
                    b.HasOne("SystemInfoApi.Models.MachineModel", "Machine")
                        .WithMany("Drives")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Machine");
                });

            modelBuilder.Entity("SystemInfoApi.Models.MachineModel", b =>
                {
                    b.HasOne("SystemInfoApi.Models.CustomerModel", "Customer")
                        .WithMany("Machines")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("SystemInfoApi.Models.OsModel", b =>
                {
                    b.HasOne("SystemInfoApi.Models.DriveModel", "Drive")
                        .WithOne("Os")
                        .HasForeignKey("SystemInfoApi.Models.OsModel", "DriveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Drive");
                });

            modelBuilder.Entity("SystemInfoApi.Models.CustomerModel", b =>
                {
                    b.Navigation("Machines");
                });

            modelBuilder.Entity("SystemInfoApi.Models.DriveModel", b =>
                {
                    b.Navigation("Os");
                });

            modelBuilder.Entity("SystemInfoApi.Models.MachineModel", b =>
                {
                    b.Navigation("Drives");
                });
#pragma warning restore 612, 618
        }
    }
}
