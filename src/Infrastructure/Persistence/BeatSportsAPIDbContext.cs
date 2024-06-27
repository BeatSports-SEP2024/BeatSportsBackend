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
public class BeatSportsAPIDbContext : DbContext, IBeatSportsDbContext
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
    public DbSet<DeviceToken> DeviceTokens { get ; set; }
    public DbSet<TimeChecking> TimeChecking { get; set; }
    public DbSet<RoomRequest> RoomRequests { get; set; }

    public BeatSportsAPIDbContext(
        DbContextOptions<BeatSportsAPIDbContext> options,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<CourtSportCategory>(entity =>
        {
            entity.HasKey(ss => new { ss.CourtSubdivisionId, ss.SportCategoryId });

            entity.HasOne(ss => ss.CourtSubdivision)
            .WithMany(ss => ss.CourtSportCategories)
            .HasForeignKey(ss => ss.CourtSubdivisionId)
            .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(ss => ss.SportCategory)
            .WithMany(ss => ss.CourtSportCategories)
            .HasForeignKey(ss => ss.SportCategoryId)
            .OnDelete(DeleteBehavior.NoAction);
        });
        builder.Entity<Court>().HasIndex(x => x.PlaceId).IsUnique();

        builder.Entity<RoomMember>(entity =>
        {
            entity.HasKey(ss => new { ss.CustomerId, ss.RoomMatchId });

            entity.HasOne(ss => ss.Customer)
            .WithMany(ss => ss.RoomMembers)
            .HasForeignKey(ss => ss.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(ss => ss.RoomMatch)
            .WithMany(ss => ss.RoomMembers)
            .HasForeignKey(ss => ss.RoomMatchId)
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

        var ownerAccountId = Guid.NewGuid();
        var owner1AccountId = Guid.NewGuid();
        var customer1AccountId = Guid.NewGuid();
        var customer2AccountId = Guid.NewGuid();
        var customer3AccountId = Guid.NewGuid();
        base.OnModelCreating(builder);
        #region Account
        builder.Entity<Account>()
            .HasData(
            new Account
            {
                Id = ownerAccountId,
                UserName = "owner1",
                Password = CreatePasswordHash("123456"),
                FirstName = "Nguyen",
                LastName = "Minh",
                DateOfBirth = DateTime.UtcNow.AddYears(-22),
                Gender = GenderEnums.Nam.ToString(),
                ProfilePictureURL = "Avatar Picture",
                Role = RoleEnums.Owner.ToString(),
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new Account
            {
                Id = owner1AccountId,
                UserName = "owner12",
                Password = CreatePasswordHash("123456"),
                FirstName = "Vu",
                LastName = "Duong",
                DateOfBirth = DateTime.UtcNow.AddYears(-22),
                Gender = GenderEnums.Nữ.ToString(),
                ProfilePictureURL = "Avatar Picture",
                Role = RoleEnums.Owner.ToString(),
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new Account
            {
                Id = customer1AccountId,
                UserName = "customer1",
                Password = CreatePasswordHash("123456"),
                FirstName = "Nguyen",
                LastName = "Vi",
                DateOfBirth = DateTime.UtcNow.AddYears(-22),
                Gender = GenderEnums.Nam.ToString(),
                ProfilePictureURL = "Avatar Picture",
                Role = RoleEnums.Customer.ToString(),
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new Account
            {
                Id = customer2AccountId,
                UserName = "customer2",
                Password = CreatePasswordHash("123456"),
                FirstName = "Nguyen",
                LastName = "Nhan",
                DateOfBirth = DateTime.UtcNow.AddYears(-22),
                Gender = GenderEnums.Nam.ToString(),
                ProfilePictureURL = "Avatar Picture",
                Role = RoleEnums.Customer.ToString(),
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new Account
            {
                Id = customer3AccountId,
                UserName = "customer3",
                Password = CreatePasswordHash("123456"),
                FirstName = "Nguyen",
                LastName = "Binh",
                DateOfBirth = DateTime.UtcNow.AddYears(-22),
                Gender = GenderEnums.Nam.ToString(),
                ProfilePictureURL = "Avatar Picture",
                Role = RoleEnums.Customer.ToString(),
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            });
        #endregion
        var customer1Id = Guid.NewGuid();
        var customer2Id = Guid.NewGuid();
        var customer3Id = Guid.NewGuid();
        #region Customer
        builder.Entity<Customer>()
            .HasData(new Customer
            {
                Id = customer1Id,
                AccountId = customer1AccountId,
                RewardPoints = 0,
            },
            new Customer
            {
                Id = customer2Id,
                AccountId = customer2AccountId,
                RewardPoints = 0,
            },
            new Customer
            {
                Id = customer3Id,
                AccountId = customer3AccountId,
                RewardPoints = 0,
            });
        #endregion
        var ownerId = Guid.NewGuid();
        var owner1Id = Guid.NewGuid();
        #region Owner
        builder.Entity<Owner>()
            .HasData(new Owner
            {
                Id = ownerId,
                AccountId = ownerAccountId,
            },
            new Owner
            {
                Id = owner1Id,
                AccountId = owner1AccountId,
            });
        #endregion
        var courtId = Guid.NewGuid();
        var court1Id = Guid.NewGuid();
        var court2Id = Guid.NewGuid();
        var court3Id = Guid.NewGuid();
        var court4Id = Guid.NewGuid();
        var court5Id = Guid.NewGuid();
        var court6Id = Guid.NewGuid();
        #region Courts
        builder.Entity<Court>()
            .HasData(new Court
            {
                Id = courtId,
                OwnerId = ownerId,
                Description = "Sân bóng mini tiêu chuẩn cao (chuẩn FiFa) với hệ thống phụ trợ (nhà thay đồ, nhà tắm, nhà vệ sinh) sạch sẽ thoáng mát duy nhất.",
                CourtName = "Sân bóng đá mini Long Trường Quận 9",
                Address = "Số 45 Bùi Xương Trạch, phường Long Trường, Quận 9, Thành phố, Thủ Đức, Thành phố Hồ Chí Minh",
                Latitude = 10.8032638,
                Longitude = 106.8112683,
                GoogleMapURLs = "https://maps.app.goo.gl/s6yWXEpDYU1DNjuF6",
                TimeStart = new TimeSpan(04, 00, 00),
                TimeEnd = new TimeSpan(23, 59, 59),
                PlaceId = "10.805515145695411, 106.81088572205702",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new Court
            {
                Id = court1Id,
                OwnerId = owner1Id,
                Description = "Sân cầu lông trang bị tiện nghi đầy đủ, giữ xe an ninh",
                CourtName = "Sân cầu lông B-ZONE 11",
                Address = "40 Đ. Số 11, Trường Thọ, Thủ Đức, Thành phố Hồ Chí Minh, Vietnam",
                Latitude = 10.8447102,
                Longitude = 106.7530548,
                GoogleMapURLs = "https://maps.app.goo.gl/cwrHGkHsM4769eSE7",
                TimeStart = new TimeSpan(05, 00, 00),
                TimeEnd = new TimeSpan(22, 00, 00),
                PlaceId = "10.845057917596483, 106.75295823555061",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new Court
            {
                Id = court2Id,
                OwnerId = owner1Id,
                Description = "Sân đẹp, cỏ xịn, đèn sáng, có chỗ để xe oto, bóng xịn",
                CourtName = "Sân bóng đá VNV",
                Address = "Đ. Số 11/Hẻm 52 Tổ 1, Khu phố 9, Thủ Đức, Thành phố Hồ Chí Minh, Vietnam",
                Latitude = 10.8445425,
                Longitude = 106.7526029,
                GoogleMapURLs = "https://maps.app.goo.gl/UUCSZm1p9ngEx7k79",
                TimeStart = new TimeSpan(14, 30, 00),
                TimeEnd = new TimeSpan(15, 30, 00),
                PlaceId = "10.844905847478088, 106.75213708986735",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new Court
            {
                Id = court3Id,
                OwnerId = owner1Id,
                Description = "Sân cầu lông hiện đại với sàn gỗ, thuận tiện cho các hoạt động thi đấu và tập luyện.",
                CourtName = "Sân cầu lông Marie Curie",
                Address = "26 Lê Quý Đôn, Phường Võ Thị Sáu, Quận 3, Thành phố Hồ Chí Minh",
                Latitude = 10.7821421,
                Longitude = 106.6902650,
                GoogleMapURLs = "https://maps.google.com/?q=26+Le+Quy+Don",
                TimeStart = new TimeSpan(05, 00, 00),
                TimeEnd = new TimeSpan(22, 00, 00),
                PlaceId = "10.786887, 106.690193",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false
            }, new Court
            {
                Id = court4Id,
                OwnerId = ownerId,
                Description = "Sân bóng chuyền ngoài trời với không gian rộng rãi, phù hợp cho cả tập luyện và thi đấu.",
                CourtName = "Sân bóng chuyền Tân Bình",
                Address = "36/5 Luy Bán Bích, Phường Tân Thới Hòa, Quận Tân Phú, Thành phố Hồ Chí Minh",
                Latitude = 10.7617930,
                Longitude = 106.6328650,
                GoogleMapURLs = "https://maps.google.com/?q=36/5+Luy+Bán+Bích",
                TimeStart = new TimeSpan(06, 00, 00),
                TimeEnd = new TimeSpan(21, 00, 00),
                PlaceId = "10.768199, 106.628938",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false
            }, new Court
            {
                Id = court5Id,
                OwnerId = owner1Id,
                Description = "Sân bóng đá lớn với cỏ nhân tạo chất lượng cao, có khán đài và hệ thống chiếu sáng tốt.",
                CourtName = "Sân bóng đá Phú Thọ",
                Address = "219 Lý Thường Kiệt, Phường 15, Quận 11, Thành phố Hồ Chí Minh",
                Latitude = 10.7675400,
                Longitude = 106.6583560,
                GoogleMapURLs = "https://maps.google.com/?q=219+Lý+Thường+Kiệt",
                TimeStart = new TimeSpan(04, 00, 00),
                TimeEnd = new TimeSpan(23, 30, 00),
                PlaceId = "10.769555, 106.663338",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false
            });
        #endregion
        var courtSubdivisionId1 = Guid.NewGuid();
        var courtSubdivisionId2 = Guid.NewGuid();
        var courtSubdivisionId3 = Guid.NewGuid();
        var courtSubdivisionId4 = Guid.NewGuid();
        var courtSubdivisionId5 = Guid.NewGuid();
        var courtSubdivisionId6 = Guid.NewGuid();
        var courtSubdivisionId7 = Guid.NewGuid();
        #region CourtSubdivision
        builder.Entity<CourtSubdivision>()
            .HasData(new CourtSubdivision
            {
                Id = courtSubdivisionId1,
                CourtId = court1Id,
                Description = "Sân cầu lông B-ZONE 11, phân cấp 1",
                ImageURL = "https://www.google.com/url?sa=i&url=https%3A%2F%2Fsieuthicaulong.vn%2Fsan-cau-long%2Fho-chi-minh%2Fclb-san-cau-long-b-zone-11-thu-duc-hcm&psig=AOvVaw0HN4hzDZNEFb3Qak2ZKR0A&ust=1718126663044000&source=images&cd=vfe&opi=89978449&ved=0CBIQjRxqFwoTCOib5tzG0YYDFQAAAAAdAAAAABAE",
                IsActive = true,
                BasePrice = 60000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new CourtSubdivision
            {
                Id = courtSubdivisionId2,
                CourtId = court1Id,
                Description = "Sân cầu lông B-ZONE 11, phân cấp 2",
                ImageURL = "https://www.google.com/url?sa=i&url=https%3A%2F%2Fsieuthicaulong.vn%2Fsan-cau-long%2Fho-chi-minh%2Fclb-san-cau-long-b-zone-11-thu-duc-hcm&psig=AOvVaw0HN4hzDZNEFb3Qak2ZKR0A&ust=1718126663044000&source=images&cd=vfe&opi=89978449&ved=0CBIQjRxqFwoTCOib5tzG0YYDFQAAAAAdAAAAABAQ",
                IsActive = true,
                BasePrice = 110000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new CourtSubdivision
            {
                Id = courtSubdivisionId3,
                CourtId = court1Id,
                Description = "Sân cầu lông B-ZONE 11, phân cấp 3",
                ImageURL = "https://www.google.com/url?sa=i&url=https%3A%2F%2Fsieuthicaulong.vn%2Fsan-cau-long%2Fho-chi-minh%2Fclb-san-cau-long-b-zone-11-thu-duc-hcm&psig=AOvVaw0HN4hzDZNEFb3Qak2ZKR0A&ust=1718126663044000&source=images&cd=vfe&opi=89978449&ved=0CBIQjRxqFwoTCOib5tzG0YYDFQAAAAAdAAAAABB4",
                IsActive = true,
                BasePrice = 110000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new CourtSubdivision
            {
                Id = courtSubdivisionId4,
                CourtId = court5Id,
                Description = "Sân bóng đá Phú Thọ , sân 1",
                ImageURL = "https://www.google.com/url?sa=i&url=https%3A%2F%2Fwww.pinterest.com%2Fpin%2F797348309003119598%2F&psig=AOvVaw3O8jui3j21qnIivkSvD8yP&ust=1718127217945000&source=images&cd=vfe&opi=89978449&ved=0CBIQjRxqFwoTCIDC_eXI0YYDFQAAAAAdAAAAABAJ",
                IsActive = true,
                BasePrice = 110000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new CourtSubdivision
            {
                Id = courtSubdivisionId5,
                CourtId = court5Id,
                Description = "Sân bóng đá Phú Thọ , sân 2",
                ImageURL = "https://duongminhfc.com/images/banners/san-bong-da-phu-tho-quan-11.jpg",
                IsActive = true,
                BasePrice = 120000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new CourtSubdivision
            {
                Id = courtSubdivisionId6,
                CourtId = court4Id,
                Description = "Sân chuyền Tân Bình , sân 1",
                ImageURL = "https://www.google.com/url?sa=i&url=https%3A%2F%2Fwww.myuc.vn%2Fson-san-bong-chuyen-tren-be-tong-xi-mang-voi-6-lop-son-co-hat-cao-su-nhu-san-us-open-chong-nut-polyurethane-va-decoturf-usa-84p.html&psig=AOvVaw0Tdhs_letBnUYzuVpHLEY2&ust=1718127751147000&source=images&cd=vfe&opi=89978449&ved=0CBIQjRxqFwoTCMDc_ePK0YYDFQAAAAAdAAAAABAJ",
                IsActive = true,
                BasePrice = 120000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new CourtSubdivision
            {
                Id = courtSubdivisionId7,
                CourtId = court4Id,
                Description = "Sân chuyền Tân Bình , sân 2",
                ImageURL = "https://www.google.com/url?sa=i&url=https%3A%2F%2Fwww.myuc.vn%2Fson-san-bong-chuyen-tren-be-tong-xi-mang-voi-6-lop-son-co-hat-cao-su-nhu-san-us-open-chong-nut-polyurethane-va-decoturf-usa-84p.html&psig=AOvVaw0Tdhs_letBnUYzuVpHLEY2&ust=1718127751147000&source=images&cd=vfe&opi=89978449&ved=0CBIQjRxqFwoTCMDc_ePK0YYDFQAAAAAdAAAAABAJ",
                IsActive = true,
                BasePrice = 120000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            });
        #endregion
        var soccerId = Guid.NewGuid();
        var volleyballId = Guid.NewGuid();
        var badmintionId = Guid.NewGuid();
        #region SportCategories
        builder.Entity<SportCategory>()
            .HasData(new SportCategory
            {
                Id = soccerId,
                Name = "Soccer",
                Description = "Sample Description",
                ImageURL = "Sample Image",
                IsActive = true,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new SportCategory
            {
                Id = volleyballId,
                Name = "Volleyball",
                Description = "Sample Description",
                ImageURL = "Sample Image",
                IsActive = true,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new SportCategory
            {
                Id = badmintionId,
                Name = "Badminton",
                Description = "Sample Description",
                ImageURL = "Sample Image",
                IsActive = true,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            });
        #endregion
        #region Court_Sport_Categories_Mapping
        builder.Entity<CourtSportCategory>()
            .HasData(new CourtSportCategory
            {
                CourtSubdivisionId = courtSubdivisionId1,
                SportCategoryId = soccerId,
            },
            new CourtSportCategory
            {
                CourtSubdivisionId = courtSubdivisionId2,
                SportCategoryId = badmintionId,
            },
            new CourtSportCategory
            {
                CourtSubdivisionId = courtSubdivisionId3,
                SportCategoryId = badmintionId,
            }
            );
        #endregion
        var discount20 = Guid.NewGuid();
        var christmas = Guid.NewGuid();
        var birthday = Guid.NewGuid();
        var lunarnewyear = Guid.NewGuid();
        #region Campaign
        builder.Entity<Campaign>()
            .HasData(new Campaign
            {
                Id = discount20,
                CourtId = court1Id,
                CampaignName = "Discount 20% base price",
                PercentDiscount = 20,
                StartDateApplying = DateTime.UtcNow,
                EndDateApplying = DateTime.UtcNow,
                QuantityOfCampaign = 20,
                CampaignImageURL = "Campaign Image 1",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new Campaign
            {
                Id = christmas,
                CourtId = court4Id,
                CampaignName = "Christmas Discount!!!",
                PercentDiscount = 25,
                StartDateApplying = DateTime.UtcNow,
                EndDateApplying = DateTime.UtcNow,
                QuantityOfCampaign = 10,
                CampaignImageURL = "Campaign Image 2",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new Campaign
            {
                Id = lunarnewyear,
                CourtId = court1Id,
                CampaignName = "Lunar New Year Discount!!!",
                PercentDiscount = 30,
                StartDateApplying = DateTime.UtcNow,
                EndDateApplying = DateTime.UtcNow,
                QuantityOfCampaign = 10,
                CampaignImageURL = "Campaign Image 3",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new Campaign
            {
                Id = birthday,
                CourtId = court2Id,
                CampaignName = "Birthday Discount!!!",
                PercentDiscount = 25,
                StartDateApplying = DateTime.UtcNow,
                EndDateApplying = DateTime.UtcNow,
                QuantityOfCampaign = 10,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            });
        #endregion

        var beginner = Guid.NewGuid();
        var medium = Guid.NewGuid();
        var expert = Guid.NewGuid();
        #region Level
        builder.Entity<Level>()
            .HasData(new Level
            {
                Id = beginner,
                LevelName = "Beginner",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new Level
            {
                Id = medium,
                LevelName = "Medium",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new Level
            {
                Id = expert,
                LevelName = "Expert",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            });
        #endregion

        var bookingId1 = Guid.NewGuid();
        var bookingId2 = Guid.NewGuid();
        var bookingId3 = Guid.NewGuid();
        var bookingId4 = Guid.NewGuid();
        #region Booking
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
            },
            new Booking
            {
                Id = bookingId4,
                CustomerId = customer1Id,
                CampaignId = lunarnewyear,
                CourtSubdivisionId = courtSubdivisionId7,
                PlayingDate = DateTime.Today.AddDays(1),
                StartTimePlaying = new TimeSpan(18, 0, 0), // 6 PM
                EndTimePlaying = new TimeSpan(20, 0, 0), // 8 PM
                BookingDate = DateTime.Today,
                TotalAmount = 1000,
                BookingStatus = "Approved",
                IsRoomBooking = false,
                IsDeposit = true
            });
        #endregion
        var timePeriodId1 = Guid.NewGuid();
        var timePeriodId2 = Guid.NewGuid();
        var timePeriodId3 = Guid.NewGuid();

        var roomMatch1 = Guid.NewGuid();
        var roomMatch2 = Guid.NewGuid();
        var roomMatch3 = Guid.NewGuid();
        var roomMatch4 = Guid.NewGuid();
        #region Room_Matches
        builder.Entity<RoomMatch>()
            .HasData(new RoomMatch
            {
                Id = roomMatch1,
                BookingId = bookingId1,
                LevelId = beginner,
                StartTimeRoom = new TimeSpan(14, 30, 00),
                EndTimeRoom = new TimeSpan(15, 30, 00),
                MaximumMember = 20,
                RuleRoom = "Rule Room Sample",
                Note = "Note Sample",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new RoomMatch
            {
                Id = roomMatch2,
                BookingId = bookingId2,
                LevelId = medium,
                StartTimeRoom = new TimeSpan(14, 30, 00),
                EndTimeRoom = new TimeSpan(15, 30, 00),
                MaximumMember = 20,
                RuleRoom = "Rule Room Sample",
                Note = "Note Sample",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new RoomMatch
            {
                Id = roomMatch3,
                BookingId = bookingId3,
                LevelId = expert,
                StartTimeRoom = new TimeSpan(15, 30, 00),
                EndTimeRoom = new TimeSpan(16, 30, 00),
                MaximumMember = 20,
                RuleRoom = "Rule Room Sample",
                Note = "Note Sample",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            },
            new RoomMatch
            {
                Id = roomMatch4,
                BookingId = bookingId4,
                LevelId = expert,
                StartTimeRoom = new TimeSpan(09, 30, 00),
                EndTimeRoom = new TimeSpan(10, 30, 00),
                MaximumMember = 20,
                RuleRoom = "Rule Room Sample",
                Note = "Note Sample",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            });
        #endregion
        #region TimePeriod      

        builder.Entity<TimePeriod>().HasData(
            new TimePeriod
            {
                Id = timePeriodId1,
                CourtSubdivisionId = courtSubdivisionId1,
                Description = "Giờ Cao Điểm",
                StartTime = new TimeSpan(17, 0, 0),
                EndTime = new TimeSpan(20, 0, 0),
                RateMultiplier = 1.5M,
            },
            new TimePeriod
            {
                Id = timePeriodId2,
                CourtSubdivisionId = courtSubdivisionId2,
                Description = "Giờ Thấp Điểm",
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(16, 0, 0),
                RateMultiplier = 0.8M,
            },
            new TimePeriod
            {
                Id = timePeriodId3,
                CourtSubdivisionId = courtSubdivisionId3,
                Description = "Giờ Bình Thường",
                StartTime = new TimeSpan(20, 30, 0),
                EndTime = new TimeSpan(23, 0, 0),
                RateMultiplier = 1.0M,
            });
        #endregion
        #region Room_Member
        builder.Entity<RoomMember>()
            .HasData(new RoomMember
            {
                CustomerId = customer1Id,
                RoomMatchId = roomMatch1,
                RoleInRoom = "Master",
            },
            new RoomMember
            {
                CustomerId = customer2Id,
                RoomMatchId = roomMatch1,
                RoleInRoom = "Member",
            },
            new RoomMember
            {
                CustomerId = customer3Id,
                RoomMatchId = roomMatch1,
                RoleInRoom = "Member",
            });
        #endregion
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
