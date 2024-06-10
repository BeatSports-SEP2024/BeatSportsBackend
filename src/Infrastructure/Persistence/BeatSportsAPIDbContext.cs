﻿using System.Reflection;
using System.Reflection.Emit;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Entities.PaymentEntity;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
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
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Level> Levels { get; set; }
    public DbSet<RoomMember> RoomMembers { get; set; }
    public DbSet<RoomMatch> RoomMatches { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<TimePeriod> TimePeriods { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }

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
        builder.Entity<Court>().HasIndex(x => x.PlaceId).IsUnique();

        builder.Entity<RoomMember>(entity =>
        {
            entity.HasKey(ss => new { ss.CustomerId, ss.Id });

            entity.HasOne(ss => ss.Customer)
            .WithMany(ss => ss.RoomMembers)
            .HasForeignKey(ss => ss.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(ss => ss.RoomMatch)
            .WithMany(ss => ss.RoomMembers)
            .HasForeignKey(ss => ss.Id)
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

    private static string CreatePasswordHash(string password)
    {
        PasswordHashingExtension.CreatePasswordHashing(
            password,
            out byte[] passwordSalt,
            out byte[] passwordHash
        );
        var passwordSaltString = Convert.ToBase64String(passwordSalt);
        var passwordHashString = Convert.ToBase64String(passwordHash);
        // Combine them into one string separated by a special character (e.g., ':')
        var combinedPassword = $"{passwordSaltString}:{passwordHashString}";
        return combinedPassword;
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

    void IBeatSportsDbContext.SaveChanges()
    {
        base.SaveChanges();
    }
}
