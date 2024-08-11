using System.Reflection;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Entities.CourtEntity.TimePeriod;
using BeatSportsAPI.Domain.Entities.PaymentEntity;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
using BeatSportsAPI.Infrastructure.Persistence.Interceptors;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
    public DbSet<BeatSportsAPI.Domain.Entities.Notification> Notifications { get; set; }
    //public DbSet<CourtSportCategory> CourtSportCategories { get; set; }
    public DbSet<CourtSubdivisionSetting> CourtSubdivisionSettings { get; set; }
    public DbSet<CourtSubdivision> CourtSubdivisions { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    //public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Level> Levels { get; set; }
    public DbSet<RoomMember> RoomMembers { get; set; }
    public DbSet<RoomMatch> RoomMatches { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<TimePeriod> TimePeriods { get; set; }
    public DbSet<TimePeriodCourtSubdivision> TimePeriodCourtSubdivisions { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }
    public DbSet<DeviceToken> DeviceTokens { get; set; }
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

        //builder.Entity<CourtSportCategory>(entity =>
        //{
        //    entity.HasKey(ss => new { ss.CourtSubdivisionId, ss.SportCategoryId });

        //    entity.HasOne(ss => ss.CourtSubdivision)
        //    .WithMany(ss => ss.CourtSportCategories)
        //    .HasForeignKey(ss => ss.CourtSubdivisionId)
        //    .OnDelete(DeleteBehavior.NoAction);

        //    entity.HasOne(ss => ss.SportCategory)
        //    .WithMany(ss => ss.CourtSportCategories)
        //    .HasForeignKey(ss => ss.SportCategoryId)
        //    .OnDelete(DeleteBehavior.NoAction);
        //});
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
        builder.Entity<TimePeriodCourtSubdivision>(entity =>
        {
            entity.HasKey(ss => new { ss.CourtSubdivisionId, ss.TimePeriodId });

            entity.HasOne(ss => ss.TimePeriod)
            .WithMany(ss => ss.TimePeriodCourtSubdivisions)
            .HasForeignKey(ss => ss.TimePeriodId)
            .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(ss => ss.CourtSubdivision)
            .WithMany(ss => ss.TimePeriodCourtSubdivision)
            .HasForeignKey(ss => ss.CourtSubdivisionId)
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
        /*
                //var sanbongdanhantao7 = "a93c57bd-f6d5-414e-a4b2-5aa269729a43";
                var sanbongdanhantao7 = Guid.NewGuid();
                //var sanbongdanhantao5 = "457c955b-857d-483d-8e54-02c87dbcffa9";
                //var sanbongdanhantao11 = "3593decc-3ace-451c-842d-3369cfe571c2";
                //var sanbongdaco7 = "31689b32-b8d8-4993-98f5-33b436b4f293";
                //var sanbongdaco11 = "089e939e-10ea-44b6-b7cd-f6d69cf6c06a";

                //var sanbongchuyencat = "41ae23f7-42fe-4a40-8c36-021dc7c1dd06";
                var sanbongchuyencat = Guid.NewGuid();
                //var sanbongchuyendat = "3fa63c52 - 80ee - 454a - a8b1 - 34e0c03633eb";
                //var sanbongchuyenximang = "0c9e0496 - e891 - 468c - aca5 - 6c09c1a8f159";
                //var sanbongchuyentrongnha = "effd5616 - ad35 - 4204 - 8c5e - 01ad289855e8";
                var sanbongchuyentrongnha = Guid.NewGuid();

                //var sancaulongtrongnha = "63998125 - 8cbd - 41b7 - 9123 - a6c7ca3ad63e";
                var sancaulongtrongnha = Guid.NewGuid();
                //var sancaulongngoaitroi = "9ce93f4d - b691 - 4622 - 95a5 - 3825916409f6";
                var sancaulongngoaitroi = Guid.NewGuid();

                // Định nghĩa các GUID cố định
                var ownerAccountId = new Guid("4a6fe7d8-efaa-4429-ada3-b8c4b5fb1d5f");
                var owner1AccountId = new Guid("bd7ee2c3-5c10-4567-9a87-d071d6f8c3b2");
                var customer1AccountId = new Guid("7e9fe0da-2abe-4e58-bdfd-5d64a6549d47");
                var customer2AccountId = new Guid("91c2f231-c3e9-4a13-a4d6-1ab2ca2c9754");
                var customer3AccountId = new Guid("9dca19fd-072c-4d2f-b7a7-1d0d273f9014");*/

        base.OnModelCreating(builder);

        #region Account
        builder.Entity<Account>()
            .HasData(
            new Account
            {
                Id = new Guid("4a6fe7d8-efaa-4429-ada3-b8c4b5fb1d5f"),
                UserName = "admin",
                Password = CreatePasswordHash("123456"),
                FirstName = "Admin",
                LastName = "Nguyen",
                DateOfBirth = DateTime.UtcNow.AddYears(-22),
                Gender = GenderEnums.Nam.ToString(),
                ProfilePictureURL = "Avatar Picture",
                Role = RoleEnums.Admin.ToString(),
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            }
            /*new Account
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
            }*/);
        #endregion

        /*       // Định nghĩa các GUID cố định cho ví
               var walletId1 = new Guid("4b6fe7d8-efaa-4429-ada3-b8c4b5fb1d1f");
               var walletId2 = new Guid("bd7ee2c3-5c10-4567-9a87-d071d6f8c312");
               var walletId3 = new Guid("7e9fe0da-2abe-4e58-bdfd-5d64a6549347");
               var walletId4 = new Guid("91c2f231-c3e9-4a13-a4d6-1ab2ca2c9765");
               var walletId5 = new Guid("9dca19fd-072c-4d2f-b7a7-1d0d273f9034");

               base.OnModelCreating(builder);

               #region Wallet
               builder.Entity<Wallet>()
                   .HasData(
                   new Wallet
                   {
                       Id = walletId1,
                       AccountId = ownerAccountId,
                       Balance = 18000000,
                       Created = DateTime.UtcNow,
                       LastModified = DateTime.UtcNow,
                       IsDelete = false,
                   },
                   new Wallet
                   {
                       Id = walletId2,
                       AccountId = owner1AccountId,
                       Balance = 182000000,
                       Created = DateTime.UtcNow,
                       LastModified = DateTime.UtcNow,
                       IsDelete = false,
                   },
                   new Wallet
                   {
                       Id = walletId3,
                       AccountId = customer1AccountId,
                       Balance = 12000000,
                       Created = DateTime.UtcNow,
                       LastModified = DateTime.UtcNow,
                       IsDelete = false,
                   },
                   new Wallet
                   {
                       Id = walletId4,
                       AccountId = customer2AccountId,
                       Balance = 13000000,
                       Created = DateTime.UtcNow,
                       LastModified = DateTime.UtcNow,
                       IsDelete = false,
                   },
                   new Wallet
                   {
                       Id = walletId5,
                       AccountId = customer3AccountId,
                       Balance = 13000000,
                       Created = DateTime.UtcNow,
                       LastModified = DateTime.UtcNow,
                       IsDelete = false,
                   });
               #endregion
               var customer1Id = new Guid("123e4567-e89b-12d3-a456-426614174100");
               var customer2Id = new Guid("123e4567-e89b-12d3-a456-426614174101");
               var customer3Id = new Guid("123e4567-e89b-12d3-a456-426614174102");
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
               // Định nghĩa các GUID cố định cho các Owner hiện tại
               var ownerId = new Guid("123e4567-e89b-12d3-a456-426614174000");
               var owner1Id = new Guid("123e4567-e89b-12d3-a456-426614174001");

               // Định nghĩa các GUID cố định cho các Owner mới
               var owner2Id = new Guid("123e4567-e89b-12d3-a456-426614174002");
               var owner3Id = new Guid("123e4567-e89b-12d3-a456-426614174003");
               var owner4Id = new Guid("123e4567-e89b-12d3-a456-426614174004");
               base.OnModelCreating(builder);

               #region Owner
               builder.Entity<Owner>()
                   .HasData(
                   new Owner
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
               var courtId = new Guid("d2642e7e-9a72-4e28-9c79-1e8e80134c8f");
               var court1Id = new Guid("ef2bd841-3214-434b-95aa-080165f5a2b2");
               var court2Id = new Guid("5ab1f835-cf9f-4847-b4a7-d0d20b183b44");
               var court3Id = new Guid("4f15e1fd-1f5c-40ef-9947-fa480a6859d1");
               var court4Id = new Guid("58b1deaf-656b-4fe0-90d8-396c5479381f");
               var court5Id = new Guid("72f0c66d-700a-4c05-9f78-8b9fdd3a7cda");
               var court6Id = new Guid("22ac3f2e-5932-4062-9daf-aebf8c95b525");
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
                       ImageUrls = "image1.jpge",
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
                       ImageUrls = "image1.jpge",
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
                       ImageUrls = "image1.jpge",
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
                       ImageUrls = "image1.jpge",
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
                       ImageUrls = "image1.jpge",
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
                       ImageUrls = "image1.jpge",
                       PlaceId = "10.769555, 106.663338",
                       Created = DateTime.UtcNow,
                       LastModified = DateTime.UtcNow,
                       IsDelete = false
                   });
               #endregion
               #region ID SAMPLE
               var courtSubdivisionId1 = new Guid("20f46754-d281-44c6-aa5c-d97ac4f3d8cb");
               var courtSubdivisionId2 = new Guid("7a62ef5e-fc97-48d3-a0a2-e9e290665f8d");
               var courtSubdivisionId3 = new Guid("e72938fe-50a0-4b5e-a898-a5cbf5b2039c");
               var courtSubdivisionId4 = new Guid("45ed7684-340d-414b-ae8c-fda358f62ac2");
               var courtSubdivisionId5 = new Guid("b5a7f639-aaa7-412d-8bde-7767489e6839");
               var courtSubdivisionId6 = new Guid("c7a8a07c-dd21-4323-bb8a-25c073fabcde");
               var courtSubdivisionId7 = new Guid("d104a1db-67e3-4351-9b3c-037ec06c245e");

              
               #endregion*/
                var courtSettings1 = new Guid("a93c57bd-f6d5-414e-a4b2-5aa269729a43");
                var courtSettings2 = new Guid("457c955b-857d-483d-8e54-02c87dbcffa9");
                var courtSettings3 = new Guid("3593decc-3ace-451c-842d-3369cfe571c2");
                var courtSettings4 = new Guid("31689b32-b8d8-4993-98f5-33b436b4f293");
                var courtSettings5 = new Guid("089e939e-10ea-44b6-b7cd-f6d69cf6c06a");
                var courtSettings6 = new Guid("41ae23f7-42fe-4a40-8c36-021dc7c1dd06");
                var courtSettings7 = new Guid("effd5616-ad35-4204-8c5e-01ad289855e8");
                var courtSettings8 = new Guid("0c9e0496-e891-468c-aca5-6c09c1a8f159");
                var courtSettings9 = new Guid("63998125-8cbd-41b7-9123-a6c7ca3ad63e");
                var courtSettings10 = new Guid("9ce93f4d-b691-4622-95a5-3825916409f6");
                var soccerId = new Guid("a781b595-6a4f-4d9a-b845-fb0f5c2c9a0a");
                var volleyballId = new Guid("4a6b05bc-fc25-45fe-abe9-11a4d9380f07");
                var badmintionId = new Guid("c01babc6-4047-47d5-bc9b-93c678b6342d");
        #region CourtSubdivisionSetting
        builder.Entity<CourtSubdivisionSetting>().HasData(
                new CourtSubdivisionSetting
                {
                    Id = courtSettings1,
                    SportCategoryId = soccerId,
                    CourtType = "Sân bóng đá nhân tạo 7",
                    ShortName = "Sân 7"
                },
                new CourtSubdivisionSetting
                {
                    Id = courtSettings2,
                    SportCategoryId = soccerId,
                    CourtType = "Sân bóng đá nhân tạo 5",
                    ShortName = "Sân 5"
                },
                new CourtSubdivisionSetting
                {
                    Id = courtSettings3,
                    SportCategoryId = soccerId,
                    CourtType = "Sân bóng đá nhân tạo 11",
                    ShortName = "Sân 11"
                },
                new CourtSubdivisionSetting
                {
                    Id = courtSettings4,
                    SportCategoryId = soccerId,
                    CourtType = "Sân bóng đá cỏ tự nhiên 7",
                    ShortName = "Sân 7"
                },
                new CourtSubdivisionSetting
                {
                    Id = courtSettings5,
                    SportCategoryId = soccerId,
                    CourtType = "Sân bóng đá cỏ tự nhiên 11",
                    ShortName = "Sân 11"
                },
                new CourtSubdivisionSetting
                {
                    Id = courtSettings6,
                    SportCategoryId = volleyballId,
                    CourtType = "Sân bóng chuyền mặt cát",
                    ShortName = "Sân chuyền"
                },
                new CourtSubdivisionSetting
                {
                    Id = courtSettings7,
                    SportCategoryId = volleyballId,
                    CourtType = "Sân bóng chuyền trong nhà",
                    ShortName = "Sân chuyền"
                },
                new CourtSubdivisionSetting
                {
                    Id = courtSettings8,
                    SportCategoryId = volleyballId,
                    CourtType = "Sân bóng chuyền xi măng",
                    ShortName = "Sân chuyền"
                },
                new CourtSubdivisionSetting
                {
                    Id = courtSettings9,
                    SportCategoryId = badmintionId,
                    CourtType = "Sân cầu lông trong nhà",
                    ShortName = "Sân cầu"
                },
                new CourtSubdivisionSetting
                {
                    Id = courtSettings10,
                    SportCategoryId = badmintionId,
                    CourtType = "Sân cầu lông ngoài trời",
                    ShortName = "Sân cầu"
                }
            );
        #endregion
       /* #region CourtSubdivision
        builder.Entity<CourtSubdivision>().HasData(
            new CourtSubdivision
            {
                Id = courtSubdivisionId4,
                CourtId = court4Id,
                CourtSubdivisionSettingId = courtSettings8,
                CourtSubdivisionName = "Sân bóng chuyền Ellen, phân cấp 1",
                CourtSubdivisionDescription = "Sân bóng chuyền xi măng",
                IsActive = true,
                BasePrice = 60000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
                CreatedStatus = CourtSubdivisionCreatedStatus.Accepted
            },
            new CourtSubdivision
            {
                Id = courtSubdivisionId5,
                CourtId = court4Id,
                CourtSubdivisionSettingId = courtSettings7,
                CourtSubdivisionName = "Sân bóng chuyền Ellen, phân cấp 2",
                CourtSubdivisionDescription = "Sân bóng chuyền trong nhà",
                IsActive = true,
                BasePrice = 60000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
                CreatedStatus = CourtSubdivisionCreatedStatus.Pending
            },
            new CourtSubdivision
            {
                Id = courtSubdivisionId1,
                CourtId = court1Id,
                CourtSubdivisionSettingId = courtSettings9,
                CourtSubdivisionName = "Sân cầu lông B-ZONE 11, phân cấp 1",
                CourtSubdivisionDescription = "Sân trong nhà",
                IsActive = true,
                BasePrice = 60000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
                CreatedStatus = CourtSubdivisionCreatedStatus.Accepted
            },
            new CourtSubdivision
            {
                Id = courtSubdivisionId2,
                CourtId = court1Id,
                CourtSubdivisionSettingId = courtSettings9,
                CourtSubdivisionName = "Sân cầu lông B-ZONE 11, phân cấp 2",
                CourtSubdivisionDescription = "Sân trong nhà",
                IsActive = true,
                BasePrice = 110000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
                CreatedStatus = CourtSubdivisionCreatedStatus.Rejected
            },
            new CourtSubdivision
            {
                Id = courtSubdivisionId3,
                CourtId = court1Id,
                CourtSubdivisionSettingId = courtSettings10,
                CourtSubdivisionName = "Sân cầu lông B-ZONE 11, phân cấp 3",
                CourtSubdivisionDescription = "Sân ngoài trời",
                IsActive = true,
                BasePrice = 110000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
                CreatedStatus = CourtSubdivisionCreatedStatus.Pending
            },
            new CourtSubdivision
            {
                Id = courtSubdivisionId6,
                CourtId = courtId,
                CourtSubdivisionSettingId = courtSettings1,
                CourtSubdivisionName = "Sân 1",
                CourtSubdivisionDescription = "Sân bóng đá nhân tạo",
                IsActive = true,
                BasePrice = 110000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
                CreatedStatus = CourtSubdivisionCreatedStatus.Accepted
            },
            new CourtSubdivision
            {
                Id = courtSubdivisionId7,
                CourtId = courtId,
                CourtSubdivisionSettingId = courtSettings1,
                CourtSubdivisionName = "Sân 2",
                CourtSubdivisionDescription = "Sân bóng đá nhân tạo",
                IsActive = true,
                BasePrice = 190000,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
                CreatedStatus = CourtSubdivisionCreatedStatus.Pending
            }
        );
        #endregion*/
        #region SportCategories
        builder.Entity<SportCategory>()
            .HasData(new SportCategory
            {
                Id = soccerId,
                Name = "Bóng đá",
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
                Name = "Bóng chuyền",
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
                Name = "Cầu lông",
                Description = "Sample Description",
                ImageURL = "Sample Image",
                IsActive = true,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            });
        #endregion
        //#region Court_Sport_Categories_Mapping
        //builder.Entity<CourtSportCategory>()
        //    .HasData(new CourtSportCategory
        //    {
        //        CourtSubdivisionId = courtSubdivisionId1,
        //        SportCategoryId = soccerId,
        //    },
        //    new CourtSportCategory
        //    {
        //        CourtSubdivisionId = courtSubdivisionId2,
        //        SportCategoryId = badmintionId,
        //    },
        //    new CourtSportCategory
        //    {
        //        CourtSubdivisionId = courtSubdivisionId3,
        //        SportCategoryId = badmintionId,
        //    }
        //    );
        //#endregion
        /*var discount20 = new Guid("d81fe96c-b8f4-4f64-b4f8-1a3bc9f41425");
        var christmas = new Guid("7f34ee57-38bc-4852-a7d6-57f1b26ed5af");
        var birthday = new Guid("45a55f14-ac7d-4e58-b9a9-c830013d07f1");
        var lunarnewyear = new Guid("9de56f74-7834-4aeb-b774-e18abc1bcedd");
        #region Campaign
        builder.Entity<Campaign>()
            .HasData(new Campaign
            {
                Id = discount20,
                CourtId = court1Id,
                CampaignName = "Discount 20% base price",
                PercentDiscount = 20,
                StartDateApplying = DateTime.UtcNow,
                EndDateApplying = DateTime.UtcNow.AddDays(4),
                MinValueApply = 40000,
                MaxValueDiscount = 20000,
                SportTypeApply = "Badminton",
                QuantityOfCampaign = 20,
                CampaignImageURL = "Campaign Image 1",
                Status = StatusEnums.Accepted,
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
                EndDateApplying = DateTime.UtcNow.AddDays(10),
                MinValueApply = 20000,
                MaxValueDiscount = 45000,
                SportTypeApply = "Soccer",
                QuantityOfCampaign = 10,
                CampaignImageURL = "Campaign Image 2",
                Status = StatusEnums.Accepted,
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
                EndDateApplying = DateTime.UtcNow.AddDays(5),
                MinValueApply = 0,
                MaxValueDiscount = 50000,
                SportTypeApply = "Volleyball",
                QuantityOfCampaign = 10,
                CampaignImageURL = "Campaign Image 3",
                Status = StatusEnums.Accepted,
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
                EndDateApplying = DateTime.UtcNow.AddDays(1),
                MinValueApply = 1,
                MaxValueDiscount = 50000,
                SportTypeApply = "All",
                QuantityOfCampaign = 10,
                CampaignImageURL = "Campaign Image 4",
                Status = StatusEnums.Accepted,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
            });
        #endregion

        var bookingId1 = new Guid("0fa91b15-e147-4a4c-931b-5a1abc2efb93");
        var bookingId2 = new Guid("22ae1f0b-3b4a-4c7b-947e-3612c4b6a8cd");
        var bookingId3 = new Guid("fba3e7b2-981f-4038-a306-7432db3ef4c6");
        var bookingId4 = new Guid("eadc2d2e-3ad3-4d6f-a4b1-55b6b233fe2e");
        var bookingId5 = new Guid("6c4099e7-0731-4f9d-90ee-fb7791040777");

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
                PlayingDate = DateTime.Today.AddDays(0),
                StartTimePlaying = new TimeSpan(20, 0, 0), // 8 PM
                EndTimePlaying = new TimeSpan(22, 0, 0), // 10 PM
                BookingDate = DateTime.Today,
                TotalAmount = 800,
                BookingStatus = "Finished",
                IsRoomBooking = false,
                IsDeposit = true
            },
            new Booking
            {
                Id = bookingId4,
                CustomerId = customer3Id,
                CampaignId = discount20,
                CourtSubdivisionId = courtSubdivisionId7,
                PlayingDate = DateTime.Today.AddDays(3),
                StartTimePlaying = new TimeSpan(20, 0, 0), // 8 PM
                EndTimePlaying = new TimeSpan(22, 0, 0), // 10 PM
                BookingDate = DateTime.Today,
                TotalAmount = 1800,
                BookingStatus = "Rejected",
                IsRoomBooking = false,
                IsDeposit = true
            },
            new Booking
            {
                Id = bookingId5,
                CustomerId = customer1Id,
                CampaignId = discount20,
                CourtSubdivisionId = courtSubdivisionId7,
                PlayingDate = DateTime.Today.AddDays(-1),
                StartTimePlaying = new TimeSpan(20, 0, 0), // 8 PM
                EndTimePlaying = new TimeSpan(22, 0, 0), // 10 PM
                BookingDate = DateTime.Today,
                TotalAmount = 1800,
                BookingStatus = "Cancel",
                IsRoomBooking = false,
                IsDeposit = false
            });
        #endregion
        var timePeriodId1 = new Guid("34fe77a7-485c-4fc4-b7c9-20f7332538f9");
        var timePeriodId2 = new Guid("88f7363a-5d6f-41f7-881c-34aa89f10eb2");
        var timePeriodId3 = new Guid("03ac6742-bc41-4ee1-8057-657f0a6c331c");

        var roomMatch1 = new Guid("a1e3c431-4f5b-4ebc-b485-82f456d012c4");
        var roomMatch2 = new Guid("ecb739f6-55a2-4318-aa17-824ed2c50e88");
        var roomMatch3 = new Guid("c7605db8-d9ab-4ab8-a1c8-14d30f955707");
        var roomMatch4 = new Guid("6e5080b0-9715-466c-9817-753d8a71169d");

        #region Room_Matches
        builder.Entity<RoomMatch>()
            .HasData(new RoomMatch
            {
                Id = roomMatch1,
                BookingId = bookingId1,
                LevelId = beginner,
                StartTimeRoom = DateTime.Now.AddHours(1).AddMinutes(30).AddMinutes(25),
                EndTimeRoom = DateTime.Now.AddHours(2).AddMinutes(30).AddMinutes(25),
                MaximumMember = 20,
                RuleRoom = "Rule Room Sample",
                Note = "Note Sample",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
                IsPrivate = false,
            },
            new RoomMatch
            {
                Id = roomMatch2,
                BookingId = bookingId2,
                LevelId = medium,
                StartTimeRoom = DateTime.Now.AddHours(2).AddMinutes(30),
                EndTimeRoom = DateTime.Now.AddHours(3).AddMinutes(30).AddMinutes(25),
                MaximumMember = 20,
                RuleRoom = "Rule Room Sample",
                Note = "Note Sample",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
                IsPrivate = false,
            },
            new RoomMatch
            {
                Id = roomMatch3,
                BookingId = bookingId3,
                LevelId = expert,
                StartTimeRoom = DateTime.Now.AddHours(1).AddMinutes(30).AddMinutes(25),
                EndTimeRoom = DateTime.Now.AddHours(2).AddMinutes(30).AddMinutes(25),
                MaximumMember = 20,
                RuleRoom = "Rule Room Sample",
                Note = "Note Sample",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsDelete = false,
                IsPrivate = false,
            });
        #endregion
        #region TimePeriod      

        *//*builder.Entity<TimePeriod>().HasData(
            new TimePeriod
            {
                Id = timePeriodId1,
                CourtId = court1Id,
                MinCancellationTime = new TimeSpan(6,0,0),
                Description = "Giờ Cao Điểm",
                StartTime = new TimeSpan(17, 0, 0),
                EndTime = new TimeSpan(20, 0, 0),
                RateMultiplier = 1.5M,
            },
            new TimePeriod
            {
                Id = timePeriodId2,
                CourtId = court2Id,
                MinCancellationTime = new TimeSpan(6, 0, 0),
                Description = "Giờ Thấp Điểm",
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(16, 0, 0),
                RateMultiplier = 0.8M,
            },
            new TimePeriod
            {
                Id = timePeriodId3,
                CourtId = court3Id,
                MinCancellationTime = new TimeSpan(6, 0, 0),
                Description = "Giờ Bình Thường",
                StartTime = new TimeSpan(20, 30, 0),
                EndTime = new TimeSpan(23, 0, 0),
                RateMultiplier = 1.0M,
            });*//*
        #endregion
        #region Room_Member
        builder.Entity<RoomMember>()
            .HasData(new RoomMember
            {
                CustomerId = customer1Id,
                RoomMatchId = roomMatch1,
                RoleInRoom = RoleInRoomEnums.Master,
            },
            new RoomMember
            {
                CustomerId = customer2Id,
                RoomMatchId = roomMatch1,
                RoleInRoom = RoleInRoomEnums.Member,
            },
            new RoomMember
            {
                CustomerId = customer3Id,
                RoomMatchId = roomMatch1,
                RoleInRoom = RoleInRoomEnums.Member,
            });
        #endregion
        var timeCheckingId1 = new Guid("11111111-1111-1111-1111-111111111111");
        var timeCheckingId2 = new Guid("22222222-2222-2222-2222-222222222222");
        var timeCheckingId3 = new Guid("33333333-3333-3333-3333-333333333333");
        var timeCheckingId4 = new Guid("44444444-4444-4444-4444-444444444444");
        var timeCheckingId5 = new Guid("55555555-5555-5555-5555-555555555555");
        var timeCheckingId6 = new Guid("66666666-6666-6666-6666-666666666666");
        var timeCheckingId7 = new Guid("77777777-7777-7777-7777-777777777777");
        #region TimeCheckings
        builder.Entity<TimeChecking>()
    .HasData(
        new TimeChecking
        {
            Id = timeCheckingId1,
            CourtSubdivisionId = courtSubdivisionId1,
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(4),
            IsLock = false,
            DateBooking = DateTime.UtcNow
        },
        new TimeChecking
        {
            Id = timeCheckingId2,
            CourtSubdivisionId = courtSubdivisionId2,
            StartTime = DateTime.UtcNow.AddHours(3),
            EndTime = DateTime.UtcNow.AddHours(5),
            IsLock = true,
            DateBooking = DateTime.UtcNow.AddDays(1)
        },
        new TimeChecking
        {
            Id = timeCheckingId3,
            CourtSubdivisionId = courtSubdivisionId3,
            StartTime = DateTime.UtcNow.AddDays(1).AddHours(2),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(4),
            IsLock = false,
            DateBooking = DateTime.UtcNow.AddDays(1)
        },
        new TimeChecking
        {
            Id = timeCheckingId4,
            CourtSubdivisionId = courtSubdivisionId4,
            StartTime = DateTime.UtcNow.AddDays(2).AddHours(2),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(4),
            IsLock = true,
            DateBooking = DateTime.UtcNow.AddDays(2)
        },
        new TimeChecking
        {
            Id = timeCheckingId5,
            CourtSubdivisionId = courtSubdivisionId5,
            StartTime = DateTime.UtcNow.AddDays(3).AddHours(1),
            EndTime = DateTime.UtcNow.AddDays(3).AddHours(3),
            IsLock = false,
            DateBooking = DateTime.UtcNow.AddDays(3)
        }
    );
        #endregion*/
        var beginner = new Guid("1a2b3c4d-5e6f-4a5b-8c2d-3e4f567a89b1");
        var medium = new Guid("2b3c4d5e-6f7a-4a5b-0d1e-2f3a4b5c6d7e");
        var expert = new Guid("3c4d5e6f-7a8b-4a5b-1c2d-3e4f5a6b7c8d");
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

        #region merchant
        builder.Entity<Merchant>().HasData(
            new Merchant
            {
                Id = new Guid("281B0F0D-1B5F-4A54-A102-BE0AEAADDAF6"),
                MerchantName = "BeatSports_AppUser",
                MerchantWebLink = "https://www.youtube.com/index", // website của beatsport
                MerchantIpnUrl = "", // chưa có sài Ipn
                MerchantReturnUrl = "exp://172.31.99.194:8081", // thực hiện thành công thì sẽ quay lại app, đường dẫn để open app
                SecretKey = "3EABD179-956C-4979-A068-01A600D7C8E7",
                IsActive = false,
            },
            new Merchant
            {
                Id = new Guid("A82CE63C-6C91-4451-8F58-863B534223D2"),
                MerchantName = "BeatSports_AppOwner",
                MerchantWebLink = "https://www.youtube.com/index", // website của beatsport
                MerchantIpnUrl = "", // chưa có sài Ipn
                MerchantReturnUrl = "exp://172.31.99.194:8081", // thực hiện thành công thì sẽ quay lại app, đường dẫn để open app
                SecretKey = "54061D89-D23D-4300-8A38-C9FDF0DF94B1",
                IsActive = false,
            }
            );
        #endregion

        #region destination
        builder.Entity<PaymentDestination>().HasData(
           new PaymentDestination
           {
               Id = new Guid("BF988D4B-58BB-48BE-8364-68A8C150CA5F"),
               DesName = "Cổng thanh toán VnPay",
               DesShortName = "VNPAY",
               DesParentId = "",
               DesLogo = "",
               SortIndex = 0,
               IsActive = false,
           },
           new PaymentDestination
           {
               Id = new Guid("4075443F-01EF-4996-83CB-D04EDF62C6C1"),
               DesName = "Cổng thanh toán Momo",
               DesShortName = "MOMO",
               DesParentId = "",
               DesLogo = "",
               SortIndex = 1,
               IsActive = false,
           },
           new PaymentDestination
           {
               Id = new Guid("06066EC3-DD51-439D-BCB6-773C69FBB396"),
               DesName = "Cổng thanh toán Zalopay",
               DesShortName = "ZALOPAY",
               DesParentId = "",
               DesLogo = "",
               SortIndex = 2,
               IsActive = false,
           }
           );
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
