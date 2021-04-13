using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaskDetection.Models;
using Microsoft.EntityFrameworkCore;
using WebMaskDetection.Models;

namespace WebMaskDetection.EntityStore
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        public DbSet<ImageStore> ImageStore { get; set; }
        public DbSet<WebCamPredictSetting> WebCamPredictSetting { get; set; }
        public DbSet<LocationSetting> LocationSetting { get; set; }
        public DbSet<WebMaskDetection.Models.IncidentReport> IncidentReport { get; set; }

        public DbSet<WebMaskDetection.Models.LocationEmail> LocationEmail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IncidentReport>()
                .HasOne(p => p.LocationSetting)
                .WithMany(b => b.IncidentReport)
                .HasForeignKey(p => p.LocationIndex);

            modelBuilder.Entity<LocationEmail>()
               .HasOne(p => p.LocationSetting)
               .WithMany(b => b.LocationEmails)
               .HasForeignKey(p => p.LocationIndex);

            modelBuilder.Entity<LocationTelegram>()
               .HasOne(p => p.LocationSetting)
               .WithMany(b => b.LocationTelegrams)
               .HasForeignKey(p => p.LocationIndex);
        }

        public DbSet<WebMaskDetection.Models.LocationTelegram> LocationTelegram { get; set; }


    }
}
