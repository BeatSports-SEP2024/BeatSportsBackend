using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Entities.PaymentEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BeatSportsAPI.Application.Common.Interfaces;
public interface IBeatSportsDbContext
{
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
    public DbSet<RefreshToken> RefreshToken { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    void SaveChanges();
}
