﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using www.pwa.Server.Data;

namespace www.pwa.Server.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210524164535_Sponsor3")]
    partial class Sponsor3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.6");

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.DeviceFlowCodes", b =>
                {
                    b.Property<string>("UserCode")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasMaxLength(50000)
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("DeviceCode")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Expiration")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SessionId")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("SubjectId")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.HasKey("UserCode");

                    b.HasIndex("DeviceCode")
                        .IsUnique();

                    b.HasIndex("Expiration");

                    b.ToTable("DeviceCodes");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.PersistedGrant", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ConsumedTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasMaxLength(50000)
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("TEXT");

                    b.Property<string>("SessionId")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("SubjectId")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.HasIndex("Expiration");

                    b.HasIndex("SubjectId", "ClientId", "Type");

                    b.HasIndex("SubjectId", "SessionId", "Type");

                    b.ToTable("PersistedGrants");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("www.pwa.Server.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("www.pwa.Server.Models.EntitySponsor", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("CentPerKm")
                        .HasColumnType("REAL");

                    b.Property<int?>("EntityID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Verified")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("EntityID");

                    b.ToTable("entitySponsors");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WalkSponsor", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("CentPerKm")
                        .HasColumnType("REAL");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Verified")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("WalkID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("WalkID");

                    b.ToTable("walkSponsors");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwClass", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("TotalEntities")
                        .HasColumnType("INTEGER");

                    b.Property<float>("TotalRuns")
                        .HasColumnType("REAL");

                    b.Property<int?>("WwwSchoolID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("WwwSchoolID");

                    b.ToTable("wwwClasses");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwCounter", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Count")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("wwwCounters");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwCounterQueue", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CounterName")
                        .HasColumnType("TEXT");

                    b.Property<int>("ValueChange")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("counterQueues");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwEntity", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Pseudonym")
                        .HasColumnType("TEXT");

                    b.Property<int>("Runs")
                        .HasColumnType("INTEGER");

                    b.Property<float>("TotalRuns")
                        .HasColumnType("REAL");

                    b.Property<int?>("WwwClassID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("WwwClassID");

                    b.ToTable("wwwEntities");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwRun", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("Distance")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.Property<int?>("WwwEntityID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("WwwEntityID");

                    b.ToTable("wwwRuns");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwSchool", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("TotalEntities")
                        .HasColumnType("INTEGER");

                    b.Property<float>("TotalRuns")
                        .HasColumnType("REAL");

                    b.Property<int?>("WwwWalkID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("WwwWalkID");

                    b.ToTable("wwwSchools");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwWalk", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Credential")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Guid")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("NextPointId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Start")
                        .HasColumnType("TEXT");

                    b.Property<float>("TotalDistance")
                        .HasColumnType("REAL");

                    b.Property<int>("TotalEntities")
                        .HasColumnType("INTEGER");

                    b.Property<float>("TotalRuns")
                        .HasColumnType("REAL");

                    b.Property<bool>("isActive")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("Guid")
                        .IsUnique();

                    b.HasIndex("NextPointId");

                    b.ToTable("wwwWalks");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwWalkData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<double>("Distance")
                        .HasColumnType("REAL");

                    b.Property<double>("Latitude")
                        .HasColumnType("REAL");

                    b.Property<double>("Longitude")
                        .HasColumnType("REAL");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Position")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("WwwWalkID")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WwwWalkID");

                    b.ToTable("WwwWalkData");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("www.pwa.Server.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("www.pwa.Server.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("www.pwa.Server.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("www.pwa.Server.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("www.pwa.Server.Models.EntitySponsor", b =>
                {
                    b.HasOne("www.pwa.Server.Models.WwwEntity", "Entity")
                        .WithMany("Sponsors")
                        .HasForeignKey("EntityID");

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WalkSponsor", b =>
                {
                    b.HasOne("www.pwa.Server.Models.WwwWalk", "Walk")
                        .WithMany("Sponsors")
                        .HasForeignKey("WalkID");

                    b.Navigation("Walk");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwClass", b =>
                {
                    b.HasOne("www.pwa.Server.Models.WwwSchool", "WwwSchool")
                        .WithMany("WwwClasses")
                        .HasForeignKey("WwwSchoolID");

                    b.Navigation("WwwSchool");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwEntity", b =>
                {
                    b.HasOne("www.pwa.Server.Models.WwwClass", "WwwClass")
                        .WithMany("WwwEntities")
                        .HasForeignKey("WwwClassID");

                    b.Navigation("WwwClass");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwRun", b =>
                {
                    b.HasOne("www.pwa.Server.Models.WwwEntity", "WwwEntity")
                        .WithMany("WwwRuns")
                        .HasForeignKey("WwwEntityID");

                    b.Navigation("WwwEntity");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwSchool", b =>
                {
                    b.HasOne("www.pwa.Server.Models.WwwWalk", "WwwWalk")
                        .WithMany("WwwSchools")
                        .HasForeignKey("WwwWalkID");

                    b.Navigation("WwwWalk");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwWalk", b =>
                {
                    b.HasOne("www.pwa.Server.Models.WwwWalkData", "NextPoint")
                        .WithMany()
                        .HasForeignKey("NextPointId");

                    b.Navigation("NextPoint");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwWalkData", b =>
                {
                    b.HasOne("www.pwa.Server.Models.WwwWalk", null)
                        .WithMany("Points")
                        .HasForeignKey("WwwWalkID");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwClass", b =>
                {
                    b.Navigation("WwwEntities");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwEntity", b =>
                {
                    b.Navigation("Sponsors");

                    b.Navigation("WwwRuns");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwSchool", b =>
                {
                    b.Navigation("WwwClasses");
                });

            modelBuilder.Entity("www.pwa.Server.Models.WwwWalk", b =>
                {
                    b.Navigation("Points");

                    b.Navigation("Sponsors");

                    b.Navigation("WwwSchools");
                });
#pragma warning restore 612, 618
        }
    }
}
