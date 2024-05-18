    using System.Reflection;
using System.Reflection.Emit;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Entities.PaymentEntity;
using BeatSportsAPI.Infrastructure.Identity;
using BeatSportsAPI.Infrastructure.Persistence.Interceptors;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.EntityFramework.Options;
using MediatR;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BeatSportsAPI.Infrastructure.Persistence;
public class BeatSportsAPIDbContext : ApiAuthorizationDbContext<ApplicationUser>, IBeatSportsDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Owner> Owners { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentDestination> PaymentsDestinations { get; set; }
    public DbSet<PaymentNotification> PaymentNotifications { get; set; }
    public DbSet<PaymentSignature> PaymentSignatures { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    public DbSet<Merchant> Merchants { get; set; }
    public DbSet<SportCategory> SportsCategories { get; set; }
    public DbSet<Court> Courts { get; set; }
    public DbSet<CourtSportCategory> CourtSportCategories { get; set; }
    public DbSet<CourtSubdivision> CourtSubdivisions { get; set; }
    public DbSet<CourtTimePeriod> CourtTimePeriods { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingDetail> BookingDetails { get; set; }

    public BeatSportsAPIDbContext(
        DbContextOptions<BeatSportsAPIDbContext> options,
        IOptions<OperationalStoreOptions> operationalStoreOptions,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options, operationalStoreOptions)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Entity<CourtSportCategory>(entity =>
        {
            entity.HasKey(ss => new { ss.CourtId, ss.SportCategoryId });

            entity.HasOne(ss => ss.Court)
            .WithMany(ss => ss.CourtCategories)
            .HasForeignKey(ss => ss.CourtId)
            .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(ss => ss.SportCategory)
            .WithMany(ss => ss.CourtSportCategories)
            .HasForeignKey(ss => ss.SportCategoryId)
            .OnDelete(DeleteBehavior.NoAction);
        });
        builder.Entity<CourtTimePeriod>(entity =>
        {
            entity.HasKey(ss => new { ss.CourtId, ss.TimePeriodId });

            entity.HasOne(ss => ss.Court)
            .WithMany(ss => ss.CourtTimePeriods)
            .HasForeignKey(ss => ss.CourtId)
            .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(ss => ss.TimePeriod)
            .WithMany(ss => ss.CourtTimePeriods)
            .HasForeignKey(ss => ss.TimePeriodId)
            .OnDelete(DeleteBehavior.NoAction);
        });
        builder.Entity<PaymentNotification>()
            .HasOne(pn => pn.Payment)
            .WithMany()
            .HasForeignKey(pn => pn.PaymentId)
            .OnDelete(DeleteBehavior.NoAction); 

        builder.Entity<DeviceFlowCodes>().HasNoKey();
        builder.Entity<PersistedGrant>().HasNoKey();
        builder.Entity<IdentityUserLogin<string>>()
        .HasKey(login => new { login.LoginProvider, login.ProviderKey });

        builder.Entity<IdentityUserLogin<string>>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(login => login.UserId)
            .IsRequired();

        builder.Entity<IdentityUserRole<string>>()
            .HasKey(p => new { p.UserId, p.RoleId });

        builder.Entity<IdentityUserToken<string>>()
        .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);

        return await base.SaveChangesAsync(cancellationToken);
    }
}
