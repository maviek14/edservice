﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<User> AppUsers { get; set; }
        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Profile>()
                .HasMany(p => p.Contracts)
                .WithOne(p => p.Principal!)
                .HasForeignKey(p => p.PrincipalID);

            modelBuilder
                .Entity<Profile>()
                .HasMany(p => p.Devices)
                .WithOne(p => p.Profile!)
                .HasForeignKey(p => p.ProfileID);

            modelBuilder
                .Entity<Contract>()
                .HasOne(c => c.Principal)
                .WithMany(p => p.Contracts)
                .HasForeignKey(c => c.PrincipalID);

/*            modelBuilder
                .Entity<Contract>()
                .HasOne(c => c.Mandatory)
                .WithMany(p => p.Contracts)
                .HasForeignKey(c => c.MandatoryID);*/

            modelBuilder
                .Entity<Contract>()
                .HasOne(c => c.Device);

            modelBuilder
                .Entity<Device>()
                .HasOne(d => d.Profile)
                .WithMany(p => p.Devices)
                .HasForeignKey(d => d.ProfileID);

            modelBuilder
                .Entity<User>()
                .HasOne(u => u.Profile);

            base.OnModelCreating(modelBuilder);
        }
    }
}
