using System.Reflection;
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

    //protected override void OnModelCreating(ModelBuilder builder)
    //{
    //    var ownerAccountId = Guid.NewGuid();
    //    var owner1AccountId = Guid.NewGuid();
    //    var customer1AccountId = Guid.NewGuid();
    //    var customer2AccountId = Guid.NewGuid();
    //    var customer3AccountId = Guid.NewGuid();
    //    base.OnModelCreating(builder);
    //    #region Account
    //    builder.Entity<Account>()
    //        .HasData(
    //        new Account
    //        {
    //            Id = ownerAccountId,
    //            UserName = "owner1",
    //            Password = CreatePasswordHash("123456"),
    //            FirstName = "Nguyen",
    //            LastName = "Minh",
    //            DateOfBirth = DateTime.UtcNow.AddYears(-22),
    //            Gender = GenderEnums.Male.ToString(),
    //            ProfilePictureURL = "Avatar Picture",
    //            Role = RoleEnums.Owner.ToString(),
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new Account
    //        {
    //            Id = owner1AccountId,
    //            UserName = "owner12",
    //            Password = CreatePasswordHash("123456"),
    //            FirstName = "Vu",
    //            LastName = "Duong",
    //            DateOfBirth = DateTime.UtcNow.AddYears(-22),
    //            Gender = GenderEnums.Male.ToString(),
    //            ProfilePictureURL = "Avatar Picture",
    //            Role = RoleEnums.Owner.ToString(),
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new Account
    //        {
    //            Id = customer1AccountId,
    //            UserName = "customer1",
    //            Password = CreatePasswordHash("123456"),
    //            FirstName = "Nguyen",
    //            LastName = "Vi",
    //            DateOfBirth = DateTime.UtcNow.AddYears(-22),
    //            Gender = GenderEnums.Male.ToString(),
    //            ProfilePictureURL = "Avatar Picture",
    //            Role = RoleEnums.Customer.ToString(),
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new Account
    //        {
    //            Id = customer2AccountId,
    //            UserName = "customer2",
    //            Password = CreatePasswordHash("123456"),
    //            FirstName = "Nguyen",
    //            LastName = "Nhan",
    //            DateOfBirth = DateTime.UtcNow.AddYears(-22),
    //            Gender = GenderEnums.Male.ToString(),
    //            ProfilePictureURL = "Avatar Picture",
    //            Role = RoleEnums.Customer.ToString(),
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new Account
    //        {
    //            Id = customer3AccountId,
    //            UserName = "customer3",
    //            Password = CreatePasswordHash("123456"),
    //            FirstName = "Nguyen",
    //            LastName = "Binh",
    //            DateOfBirth = DateTime.UtcNow.AddYears(-22),
    //            Gender = GenderEnums.Male.ToString(),
    //            ProfilePictureURL = "Avatar Picture",
    //            Role = RoleEnums.Customer.ToString(),
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        });
    //    #endregion
    //    var customer1Id = Guid.NewGuid();
    //    var customer2Id = Guid.NewGuid();
    //    var customer3Id = Guid.NewGuid();
    //    #region Customer
    //    builder.Entity<Customer>()
    //        .HasData(new Customer
    //        {
    //            Id = customer1Id,
    //            AccountId = customer1AccountId,
    //            RewardPoints = 0,
    //        },
    //        new Customer
    //        {
    //            Id = customer2Id,
    //            AccountId = customer2AccountId,
    //            RewardPoints = 0,
    //        },
    //        new Customer
    //        {
    //            Id = customer3Id,
    //            AccountId = customer3AccountId,
    //            RewardPoints = 0,
    //        });
    //    #endregion
    //    var ownerId = Guid.NewGuid();
    //    var owner1Id = Guid.NewGuid();
    //    #region Owner
    //    builder.Entity<Owner>()
    //        .HasData(new Owner
    //        {
    //            Id = ownerId,
    //            AccountId = ownerAccountId,
    //        },
    //        new Owner
    //        {
    //            Id = owner1Id,
    //            AccountId = owner1AccountId,
    //        });
    //    #endregion
    //    var courtId = Guid.NewGuid();
    //    var court1Id = Guid.NewGuid();
    //    var court2Id = Guid.NewGuid();
    //    #region Courts
    //    builder.Entity<Court>()
    //        .HasData(new Court
    //        {
    //            Id = courtId,
    //            OwnerId = ownerId,
    //            Description = "Description Sample",
    //            CourtName = "Court Name Sample",
    //            Address = "Address Sample",
    //            GoogleMapURLs = "Address Sample",
    //            TimeStart = new TimeSpan(14, 30, 00),
    //            TimeEnd = new TimeSpan(15, 30, 00),
    //            PlaceId = "From Google Map",
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new Court
    //        {
    //            Id = court1Id,
    //            OwnerId = owner1Id,
    //            Description = "Description Sample",
    //            CourtName = "Court Name Sample",
    //            Address = "Address Sample",
    //            GoogleMapURLs = "Address Sample",
    //            TimeStart = new TimeSpan(14, 30, 00),
    //            TimeEnd = new TimeSpan(15, 30, 00),
    //            PlaceId = "From Google Map",
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new Court
    //        {
    //            Id = court2Id,
    //            OwnerId = owner1Id,
    //            Description = "Description Sample",
    //            CourtName = "Court Name Sample",
    //            Address = "Address Sample",
    //            GoogleMapURLs = "Address Sample",
    //            TimeStart = new TimeSpan(14, 30, 00),
    //            TimeEnd = new TimeSpan(15, 30, 00),
    //            PlaceId = "From Google Map",
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        });
    //    #endregion
    //    var courtSubdivisionId1 = Guid.NewGuid();
    //    var courtSubdivisionId2 = Guid.NewGuid();
    //    var courtSubdivisionId3 = Guid.NewGuid();
    //    #region CourtSubdivision
    //    builder.Entity<CourtSubdivision>()
    //        .HasData(new CourtSubdivision
    //        {
    //            Id = courtSubdivisionId1,
    //            CourtId = court1Id,
    //            Description = "Sample Description",
    //            ImageURL = "Sample Image",
    //            IsActive = true,
    //            BasePrice = 120000,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new CourtSubdivision
    //        {
    //            Id = courtSubdivisionId2,
    //            CourtId = court1Id,
    //            Description = "Sample Description",
    //            ImageURL = "Sample Image",
    //            IsActive = true,
    //            BasePrice = 120000,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new CourtSubdivision
    //        {
    //            Id = courtSubdivisionId3,
    //            CourtId = court1Id,
    //            Description = "Sample Description",
    //            ImageURL = "Sample Image",
    //            IsActive = true,
    //            BasePrice = 120000,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        });
    //    #endregion
    //    var soccerId = Guid.NewGuid();
    //    var volleyballId = Guid.NewGuid();
    //    var badmintionId = Guid.NewGuid();
    //    #region SportCategories
    //    builder.Entity<SportCategory>()
    //        .HasData(new SportCategory
    //        {
    //            Id = soccerId,
    //            Name = "Soccer",
    //            Description = "Sample Description",
    //            ImageURL = "Sample Image",
    //            IsActive = true,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new SportCategory
    //        {
    //            Id = volleyballId,
    //            Name = "Volleyball",
    //            Description = "Sample Description",
    //            ImageURL = "Sample Image",
    //            IsActive = true,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new SportCategory
    //        {
    //            Id = badmintionId,
    //            Name = "Badminton",
    //            Description = "Sample Description",
    //            ImageURL = "Sample Image",
    //            IsActive = true,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        });
    //    #endregion
    //    #region Court_Sport_Categories_Mapping
    //    builder.Entity<CourtSportCategory>()
    //        .HasData(new CourtSportCategory
    //        {
    //            Id = Guid.NewGuid(),
    //            CourtId = court1Id,
    //            SportCategoryId = soccerId,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new CourtSportCategory
    //        {
    //            Id = Guid.NewGuid(),
    //            CourtId = courtId,
    //            SportCategoryId = soccerId,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new CourtSportCategory
    //        {
    //            Id = Guid.NewGuid(),
    //            CourtId = court1Id,
    //            SportCategoryId = badmintionId,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new CourtSportCategory
    //        {
    //            Id = Guid.NewGuid(),
    //            CourtId = court1Id,
    //            SportCategoryId = badmintionId,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        });
    //    #endregion
    //    var discount20 = Guid.NewGuid();
    //    var christmas = Guid.NewGuid();
    //    var lunarnewyear = Guid.NewGuid();
    //    #region Campaign
    //    builder.Entity<Campaign>()
    //        .HasData(new Campaign
    //        {
    //            Id = discount20,
    //            CourtId = court1Id,
    //            CampaignName = "Discount 20% base price",
    //            PercentDiscount = 20,
    //            StartDateApplying = DateTime.UtcNow,
    //            EndDateApplying = DateTime.UtcNow,
    //            QuantityOfCampaign = 20,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new Campaign
    //        {
    //            Id = christmas,
    //            CourtId = court1Id,
    //            CampaignName = "Christmas Discount!!!",
    //            PercentDiscount = 25,
    //            StartDateApplying = DateTime.UtcNow,
    //            EndDateApplying = DateTime.UtcNow,
    //            QuantityOfCampaign = 10,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new Campaign
    //        {
    //            Id = lunarnewyear,
    //            CourtId = court1Id,
    //            CampaignName = "Lunar New Year Discount!!!",
    //            PercentDiscount = 30,
    //            StartDateApplying = DateTime.UtcNow,
    //            EndDateApplying = DateTime.UtcNow,
    //            QuantityOfCampaign = 10,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        });
    //    #endregion

    //    var beginner = Guid.NewGuid();
    //    var medium = Guid.NewGuid();
    //    var expert = Guid.NewGuid();
    //    #region Level
    //    builder.Entity<Level>()
    //        .HasData(new Level
    //        {
    //            Id = beginner,
    //            LevelName = "Beginner",
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new Level
    //        {
    //            Id = medium,
    //            LevelName = "Medium",
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new Level
    //        {
    //            Id = expert,
    //            LevelName = "Expert",
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        });
    //    #endregion

    //    var bookingId1 = Guid.NewGuid();
    //    var bookingId2 = Guid.NewGuid();
    //    var bookingId3 = Guid.NewGuid();

        builder.Entity<Booking>().HasData(
            new Booking
            {
                Id = bookingId1,
                CustomerId = customer1Id,
                CampaignId = discount20,
                CourtSubdivisionId = courtSubdivisionId1,
                PlayingDate = DateTime.Today.AddDays(1),
                StartTimePlaying = new TimeSpan(18, 0, 0), // 6 PM
                EndTimePlaying = new TimeSpan(20, 0, 0), // 8 PM
                BookingDate = DateTime.Today,
                TotalAmount = 1000,
                BookingStatus = "Approved",
                IsRoomBooking = false,
                IsDeposit = true
            },
            new Booking
            {
                Id = bookingId2,
                CustomerId = customer2Id,
                CampaignId = christmas,
                CourtSubdivisionId = courtSubdivisionId2,
                PlayingDate = DateTime.Today.AddDays(2),
                StartTimePlaying = new TimeSpan(17, 0, 0), // 5 PM
                EndTimePlaying = new TimeSpan(19, 0, 0), // 7 PM
                BookingDate = DateTime.Today,
                TotalAmount = 1200,
                BookingStatus = "Approved",
                IsRoomBooking = true,
                IsDeposit = false
            },
            new Booking
            {
                Id = bookingId3,
                CustomerId = customer3Id,
                CampaignId = lunarnewyear,
                CourtSubdivisionId = courtSubdivisionId3,
                PlayingDate = DateTime.Today.AddDays(3),
                StartTimePlaying = new TimeSpan(20, 0, 0), // 8 PM
                EndTimePlaying = new TimeSpan(22, 0, 0), // 10 PM
                BookingDate = DateTime.Today,
                TotalAmount = 800,
                BookingStatus = "Approved",
                IsRoomBooking = false,
                IsDeposit = true
            });
        var timePeriodId1 = Guid.NewGuid();
        var timePeriodId2 = Guid.NewGuid();
        var timePeriodId3 = Guid.NewGuid();

    //    var roomMatch1 = Guid.NewGuid();
    //    var roomMatch2 = Guid.NewGuid();
    //    var roomMatch3 = Guid.NewGuid();
    //    #region Room_Matches
    //    builder.Entity<RoomMatch>()
    //        .HasData(new RoomMatch
    //        {
    //            Id = roomMatch1,
    //            BookingId = bookingId1,
    //            LevelId = beginner,
    //            CourtSubdivisionId = courtSubdivisionId1,
    //            StartTimeRoom = new TimeSpan(14, 30, 00),
    //            EndTimeRoom = new TimeSpan(15, 30, 00),
    //            MaximumMember = 20,
    //            RuleRoom = "Rule Room Sample",
    //            Note = "Note Sample",
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new RoomMatch
    //        {
    //            Id = roomMatch2,
    //            BookingId = bookingId2,
    //            LevelId = medium,
    //            CourtSubdivisionId = courtSubdivisionId2,
    //            StartTimeRoom = new TimeSpan(14, 30, 00),
    //            EndTimeRoom = new TimeSpan(15, 30, 00),
    //            MaximumMember = 20,
    //            RuleRoom = "Rule Room Sample",
    //            Note = "Note Sample",
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        },
    //        new RoomMatch
    //        {
    //            Id = roomMatch3,
    //            BookingId = bookingId3,
    //            LevelId = expert,
    //            CourtSubdivisionId = courtSubdivisionId3,
    //            StartTimeRoom = new TimeSpan(14, 30, 00),
    //            EndTimeRoom = new TimeSpan(15, 30, 00),
    //            MaximumMember = 20,
    //            RuleRoom = "Rule Room Sample",
    //            Note = "Note Sample",
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            IsDelete = false,
    //        });
    //    #endregion
    //    #region TimePeriod      
    //    var timePeriodId1 = Guid.NewGuid();
    //    var timePeriodId2 = Guid.NewGuid();
    //    var timePeriodId3 = Guid.NewGuid();

    //    builder.Entity<TimePeriod>().HasData(
    //        new TimePeriod
    //        {
    //            Id = timePeriodId1,
    //            CourtSubdivisionId = courtSubdivisionId1,
    //            Description = "Giờ Cao Điểm",
    //            StartTime = new TimeSpan(17, 0, 0),
    //            EndTime = new TimeSpan(20, 0, 0),
    //            RateMultiplier = 1.5M,
    //        },
    //        new TimePeriod
    //        {
    //            Id = timePeriodId2,
    //            CourtSubdivisionId = courtSubdivisionId2,
    //            Description = "Giờ Thấp Điểm",
    //            StartTime = new TimeSpan(10, 0, 0),
    //            EndTime = new TimeSpan(16, 0, 0),
    //            RateMultiplier = 0.8M,
    //        },
    //        new TimePeriod
    //        {
    //            Id = timePeriodId3,
    //            CourtSubdivisionId = courtSubdivisionId3,
    //            Description = "Giờ Bình Thường",
    //            StartTime = new TimeSpan(20, 30, 0),
    //            EndTime = new TimeSpan(23, 0, 0),
    //            RateMultiplier = 1.0M,
    //        });
    //    #endregion
    //    #region Room_Member
    //    builder.Entity<RoomMember>()
    //        .HasData(new RoomMember
    //        {
    //            Id = Guid.NewGuid(),
    //            CustomerId = customer1Id,
    //            RoomMatchId = roomMatch1,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            RoleInRoom = "Master",
    //            IsDelete = false,
    //        },
    //        new RoomMember
    //        {
    //            Id = Guid.NewGuid(),
    //            CustomerId = customer2Id,
    //            RoomMatchId = roomMatch1,
    //            Created = DateTime.UtcNow,
    //            LastModified = DateTime.UtcNow,
    //            RoleInRoom = "Member",
    //            IsDelete = false,
    //        });
    //    #endregion
    //}

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
