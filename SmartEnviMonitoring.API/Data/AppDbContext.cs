using Microsoft.EntityFrameworkCore;
using SmartEnviMonitoring.API.Data.Monitoring;
using SmartEnviMonitoring.API.Data.System;


namespace SmartEnviMonitoring.Data;

public partial class AppDbContext : DbContext
{
    public DbSet<MeasurementRecord> MeasRecords { get; set; }
    public DbSet<AudioRecord> AudioRecords { get; set; }
    public DbSet<SystemEvent> SystemEvents { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<MonitoringDevice> Devices { get; set; }
    public DbSet<ApiUser> Users {  get; set; }
    public AppDbContext()
    {

    }
    public AppDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MeasurementRecord>(entity =>
        {
            entity.HasIndex(e => e.Id).IsUnique();
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}