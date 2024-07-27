using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BeatSportsAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GoogleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilePictureURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceFlowCodes",
                columns: table => new
                {
                    DeviceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "IdentityUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserRole<string>",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRole<string>", x => new { x.UserId, x.RoleId });
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserToken<string>",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserToken<string>", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Levels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LevelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LevelDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Levels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Merchants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantWebLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantIpnUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantReturnUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecretKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentsDestinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DesName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesParentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortIndex = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentsDestinations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersistedGrant",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConsumedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "SportsCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportsCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RewardPoints = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "DeviceTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceTokens_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Owners_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TokenExpires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserLogin<string>",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogin<string>", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_IdentityUserLogin<string>_IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentDestinationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentRefId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiredAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentLanguage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentLastMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Payments_Merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "Merchants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentsDestinations_PaymentDestinationId",
                        column: x => x.PaymentDestinationId,
                        principalTable: "PaymentsDestinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "CourtSubdivisionSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SportCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourtType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtSubdivisionSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourtSubdivisionSettings_SportsCategories_SportCategoryId",
                        column: x => x.SportCategoryId,
                        principalTable: "SportsCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Courts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CourtName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    WallpaperUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CourtAvatarImgUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatitudeDelta = table.Column<double>(type: "float", nullable: true),
                    LongitudeDelta = table.Column<double>(type: "float", nullable: true),
                    GoogleMapURLs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeStart = table.Column<TimeSpan>(type: "time", nullable: false),
                    TimeEnd = table.Column<TimeSpan>(type: "time", nullable: false),
                    ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlaceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courts_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletTargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminCheckStatus = table.Column<int>(type: "int", nullable: true),
                    TransactionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "PaymentNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentRefId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotiDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NotiContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotiAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NotiMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotiSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotiNotiStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotiResDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NotiResMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotiResHttpCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentNotifications_Merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "Merchants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_PaymentNotifications_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentSignatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SignValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignAlgo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignOwn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSignatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentSignatures_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TranMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TranPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TranStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TranAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TranDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TranRefId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourtId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PercentDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SportTypeApply = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinValueApply = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxValueDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDateApplying = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateApplying = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    QuantityOfCampaign = table.Column<int>(type: "int", nullable: false),
                    CampaignImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonOfReject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaigns_Courts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "CourtSubdivisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourtId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourtSubdivisionSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CourtSubdivisionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CourtSubdivisionDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedStatus = table.Column<int>(type: "int", nullable: false),
                    ReasonOfRejected = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtSubdivisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourtSubdivisions_CourtSubdivisionSettings_CourtSubdivisionSettingId",
                        column: x => x.CourtSubdivisionSettingId,
                        principalTable: "CourtSubdivisionSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CourtSubdivisions_Courts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TimePeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourtId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    RateMultiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsRefundDeposit = table.Column<bool>(type: "bit", nullable: false),
                    PercentDeposit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimePeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimePeriods_Courts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourtSubdivisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsRoomBooking = table.Column<bool>(type: "bit", nullable: false),
                    IsDeposit = table.Column<bool>(type: "bit", nullable: false),
                    PlayingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTimePlaying = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTimePlaying = table.Column<TimeSpan>(type: "time", nullable: false),
                    BookingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bookings_CourtSubdivisions_CourtSubdivisionId",
                        column: x => x.CourtSubdivisionId,
                        principalTable: "CourtSubdivisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Bookings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TimeChecking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourtSubdivisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsLock = table.Column<bool>(type: "bit", nullable: false),
                    DateBooking = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeChecking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeChecking_CourtSubdivisions_CourtSubdivisionId",
                        column: x => x.CourtSubdivisionId,
                        principalTable: "CourtSubdivisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourtId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeedbackStar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FeedbackAvailable = table.Column<bool>(type: "bit", nullable: false),
                    FeedbackStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeedbackContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Courts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "RoomMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SportCategory = table.Column<int>(type: "int", nullable: false),
                    StartTimeRoom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTimeRoom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaximumMember = table.Column<int>(type: "int", nullable: false),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RuleRoom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomMatches_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_RoomMatches_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "RoomMembers",
                columns: table => new
                {
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomMatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleInRoom = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomMembers", x => new { x.CustomerId, x.RoomMatchId });
                    table.ForeignKey(
                        name: "FK_RoomMembers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoomMembers_RoomMatches_RoomMatchId",
                        column: x => x.RoomMatchId,
                        principalTable: "RoomMatches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoomRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomMatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JoinStatus = table.Column<int>(type: "int", nullable: false),
                    DateRequest = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateApprove = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomRequests_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_RoomRequests_RoomMatches_RoomMatchId",
                        column: x => x.RoomMatchId,
                        principalTable: "RoomMatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Bio", "Created", "CreatedBy", "DateOfBirth", "Email", "FirstName", "Gender", "GoogleId", "IsDelete", "LastModified", "LastModifiedBy", "LastName", "Password", "PhoneNumber", "ProfilePictureURL", "Role", "UserName" },
                values: new object[,]
                {
                    { new Guid("4a6fe7d8-efaa-4429-ada3-b8c4b5fb1d5f"), null, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4093), null, new DateTime(2002, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(3970), null, "Nguyen", "Nam", null, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4093), null, "Minh", "+mxD+BN4RulMK9+5cNGQCXMjv7QLodaU2+cuhaAyV+CyB3aBgvJgL9fsIFGAjztemOql42R54qtqEglXohxH5DrOFNSBLiUxezAI0BmdgkbxK5nEX56C3Ah0JRz00m72o2g4woGFYAwXJLjMQA9n3usmoKbdmqnaKvXKJmWncdo=:SYEwsRfpkj8lenWTIAg/9T8VfvDCq1OUdjoroDDOmQhb2j3+OSsxnYzgoboMdCsysWZ56W5xbUeIS1cwiM0WyA==", null, "Avatar Picture", "Owner", "owner1" },
                    { new Guid("7e9fe0da-2abe-4e58-bdfd-5d64a6549d47"), null, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4181), null, new DateTime(2002, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4178), null, "Nguyen", "Nam", null, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4182), null, "Vi", "fnfZspRGRD2NtVjzXomipxQlPzPRPqIl1sRNJ+2+UGlRLGQRE7tNlmHfs2A+BNuPDPxz9XVvUSn9G6EO5i5uB3FIVK52uQ3gQH0XTezlGJncmMwO3QHAwjALYiW2x66P8mCqL97uagL9RwdGZ0GAptktAnCnKuK9AaQ9jlBSvK4=:Ns6pMfCmRHquWFgkzwpy9v4XJ3HsOY2vIhip6G2FK39YoDreGtISJBlm9viI2ZFh1RrfZmtzswL+nDVc1kPLbw==", null, "Avatar Picture", "Customer", "customer1" },
                    { new Guid("91c2f231-c3e9-4a13-a4d6-1ab2ca2c9754"), null, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4215), null, new DateTime(2002, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4212), null, "Nguyen", "Nam", null, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4215), null, "Nhan", "Dy0i4Ar15F4O0ioZE/fJWwrW9zOg1SJRBYRFX81nHPOq5j2D1FxkZO6PE365RBZ2VKf8SMdbjpRxN6uYbst1m53swwJHerJdeI1Ol8sJ2Ya1Uwd3ECVfQOH1R/98IbU2MHmp6Hydvpwhlp2WkKV+9+dDGQwZC7C2kcXuTQBMQ4s=:9DerGXU5hplkG7gAdx9UURtyNLCReHLzHlUftHmfQ6+McWI3fftRqKUwRtwQI5bwHoZC7Nz9rS7RrP2u/656ww==", null, "Avatar Picture", "Customer", "customer2" },
                    { new Guid("9dca19fd-072c-4d2f-b7a7-1d0d273f9014"), null, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4254), null, new DateTime(2002, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4251), null, "Nguyen", "Nam", null, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4254), null, "Binh", "juX5qVpiU8Wfy37zSNhZdUeytAKkqztpjZgNoELf9ufUILkIG0IPYlM05I2dR8RK+0BslFAkujffUpmqXsK0AspDo8YObN4w6diVml2ZCTc7pxAHK6UdTYVMqwMPpo8F3S6DzZBmY3ria7PxNrr1mv3GwDkxLPJfJVsfqplEdV8=:Tn5Zg/V55bzgCHVcfMjKqyigp+rOEqc5AIhXZhLh1T1yUW6vJioXXyXk1+XjP+pykp6/s34iyx2OW6GrwcFlIA==", null, "Avatar Picture", "Customer", "customer3" },
                    { new Guid("bd7ee2c3-5c10-4567-9a87-d071d6f8c3b2"), null, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4146), null, new DateTime(2002, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4143), null, "Vu", "Nữ", null, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4147), null, "Duong", "o2Im2JNNQFkz9Y81wTDlsQi1BQ2ieC1MZBS8LipCKktwpZ4qdjfAOVketYYSmfCzWRJRS8ADbBv6dWrB8iNMLBLzO3pfnqMzEwrf+HxjZBPHpak45b70+PTXwdWxRaeriftYNPwGJw6pv/BDkcMSfN4YlogNqatb6I/D5jaHOJM=:aYcFlx9lf74mWR6Or880XR2H9ULUouiqRh7ctz+ke9r5LIQ6qR1Atpzx65mBPez0JHztAv3/XoUB4HI3KfEccg==", null, "Avatar Picture", "Owner", "owner12" }
                });

            migrationBuilder.InsertData(
                table: "Levels",
                columns: new[] { "Id", "Created", "CreatedBy", "IsDelete", "LastModified", "LastModifiedBy", "LevelDescription", "LevelName" },
                values: new object[,]
                {
                    { new Guid("1a2b3c4d-5e6f-4a5b-8c2d-3e4f567a89b1"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4939), null, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4939), null, null, "Beginner" },
                    { new Guid("2b3c4d5e-6f7a-4a5b-0d1e-2f3a4b5c6d7e"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4941), null, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4942), null, null, "Medium" },
                    { new Guid("3c4d5e6f-7a8b-4a5b-1c2d-3e4f5a6b7c8d"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4943), null, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4944), null, null, "Expert" }
                });

            migrationBuilder.InsertData(
                table: "Merchants",
                columns: new[] { "Id", "Created", "CreatedBy", "IsActive", "IsDelete", "LastModified", "LastModifiedBy", "MerchantIpnUrl", "MerchantName", "MerchantReturnUrl", "MerchantWebLink", "SecretKey" },
                values: new object[,]
                {
                    { new Guid("281b0f0d-1b5f-4a54-a102-be0aeaaddaf6"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5149), null, false, false, null, null, "", "BeatSports_AppUser", "exp://172.31.99.194:8081", "https://www.youtube.com/index", "3EABD179-956C-4979-A068-01A600D7C8E7" },
                    { new Guid("a82ce63c-6c91-4451-8f58-863b534223d2"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5155), null, false, false, null, null, "", "BeatSports_AppOwner", "exp://172.31.99.194:8081", "https://www.youtube.com/index", "54061D89-D23D-4300-8A38-C9FDF0DF94B1" }
                });

            migrationBuilder.InsertData(
                table: "PaymentsDestinations",
                columns: new[] { "Id", "Created", "CreatedBy", "DesLogo", "DesName", "DesParentId", "DesShortName", "IsActive", "IsDelete", "LastModified", "LastModifiedBy", "SortIndex" },
                values: new object[,]
                {
                    { new Guid("06066ec3-dd51-439d-bcb6-773c69fbb396"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5185), null, "", "Cổng thanh toán Zalopay", "", "ZALOPAY", false, false, null, null, 2 },
                    { new Guid("281b0f0d-1b5f-4a54-a102-be0aeaaddaf6"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5178), null, "", "Cổng thanh toán VnPay", "", "VNPAY", false, false, null, null, 0 },
                    { new Guid("4075443f-01ef-4996-83cb-d04edf62c6c1"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5183), null, "", "Cổng thanh toán Momo", "", "MOMO", false, false, null, null, 1 }
                });

            migrationBuilder.InsertData(
                table: "SportsCategories",
                columns: new[] { "Id", "Created", "CreatedBy", "Description", "ImageURL", "IsActive", "IsDelete", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("4a6b05bc-fc25-45fe-abe9-11a4d9380f07"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4871), null, "Sample Description", "Sample Image", true, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4871), null, "Bóng chuyền" },
                    { new Guid("a781b595-6a4f-4d9a-b845-fb0f5c2c9a0a"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4868), null, "Sample Description", "Sample Image", true, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4868), null, "Bóng đá" },
                    { new Guid("c01babc6-4047-47d5-bc9b-93c678b6342d"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4873), null, "Sample Description", "Sample Image", true, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4873), null, "Cầu lông" }
                });

            migrationBuilder.InsertData(
                table: "CourtSubdivisionSettings",
                columns: new[] { "Id", "CourtType", "Created", "CreatedBy", "IsDelete", "LastModified", "LastModifiedBy", "ShortName", "SportCategoryId" },
                values: new object[,]
                {
                    { new Guid("089e939e-10ea-44b6-b7cd-f6d69cf6c06a"), "Sân bóng đá cỏ tự nhiên 11", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4782), null, false, null, null, null, new Guid("a781b595-6a4f-4d9a-b845-fb0f5c2c9a0a") },
                    { new Guid("0c9e0496-e891-468c-aca5-6c09c1a8f159"), "Sân bóng chuyền xi măng", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4792), null, false, null, null, null, new Guid("4a6b05bc-fc25-45fe-abe9-11a4d9380f07") },
                    { new Guid("31689b32-b8d8-4993-98f5-33b436b4f293"), "Sân bóng đá cỏ tự nhiên 7", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4777), null, false, null, null, null, new Guid("a781b595-6a4f-4d9a-b845-fb0f5c2c9a0a") },
                    { new Guid("3593decc-3ace-451c-842d-3369cfe571c2"), "Sân bóng đá nhân tạo 11", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4776), null, false, null, null, null, new Guid("a781b595-6a4f-4d9a-b845-fb0f5c2c9a0a") },
                    { new Guid("41ae23f7-42fe-4a40-8c36-021dc7c1dd06"), "Sân bóng chuyền mặt cát", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4783), null, false, null, null, null, new Guid("4a6b05bc-fc25-45fe-abe9-11a4d9380f07") },
                    { new Guid("457c955b-857d-483d-8e54-02c87dbcffa9"), "Sân bóng đá nhân tạo 5", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4774), null, false, null, null, null, new Guid("a781b595-6a4f-4d9a-b845-fb0f5c2c9a0a") },
                    { new Guid("63998125-8cbd-41b7-9123-a6c7ca3ad63e"), "Sân cầu lông trong nhà", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4794), null, false, null, null, null, new Guid("c01babc6-4047-47d5-bc9b-93c678b6342d") },
                    { new Guid("9ce93f4d-b691-4622-95a5-3825916409f6"), "Sân cầu lông ngoài trời", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4795), null, false, null, null, null, new Guid("c01babc6-4047-47d5-bc9b-93c678b6342d") },
                    { new Guid("a93c57bd-f6d5-414e-a4b2-5aa269729a43"), "Sân bóng đá nhân tạo 7", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4772), null, false, null, null, null, new Guid("a781b595-6a4f-4d9a-b845-fb0f5c2c9a0a") },
                    { new Guid("effd5616-ad35-4204-8c5e-01ad289855e8"), "Sân bóng chuyền trong nhà", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4785), null, false, null, null, null, new Guid("4a6b05bc-fc25-45fe-abe9-11a4d9380f07") }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "AccountId", "Address", "Created", "CreatedBy", "IsDelete", "LastModified", "LastModifiedBy", "RewardPoints" },
                values: new object[,]
                {
                    { new Guid("123e4567-e89b-12d3-a456-426614174100"), new Guid("7e9fe0da-2abe-4e58-bdfd-5d64a6549d47"), null, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4651), null, false, null, null, 0 },
                    { new Guid("123e4567-e89b-12d3-a456-426614174101"), new Guid("91c2f231-c3e9-4a13-a4d6-1ab2ca2c9754"), null, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4654), null, false, null, null, 0 },
                    { new Guid("123e4567-e89b-12d3-a456-426614174102"), new Guid("9dca19fd-072c-4d2f-b7a7-1d0d273f9014"), null, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4655), null, false, null, null, 0 }
                });

            migrationBuilder.InsertData(
                table: "Owners",
                columns: new[] { "Id", "AccountId", "Address", "Created", "CreatedBy", "IsDelete", "LastModified", "LastModifiedBy" },
                values: new object[,]
                {
                    { new Guid("123e4567-e89b-12d3-a456-426614174000"), new Guid("4a6fe7d8-efaa-4429-ada3-b8c4b5fb1d5f"), null, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4678), null, false, null, null },
                    { new Guid("123e4567-e89b-12d3-a456-426614174001"), new Guid("bd7ee2c3-5c10-4567-9a87-d071d6f8c3b2"), null, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4680), null, false, null, null }
                });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "AccountId", "Balance", "Created", "CreatedBy", "IsDelete", "LastModified", "LastModifiedBy" },
                values: new object[,]
                {
                    { new Guid("4b6fe7d8-efaa-4429-ada3-b8c4b5fb1d1f"), new Guid("4a6fe7d8-efaa-4429-ada3-b8c4b5fb1d5f"), 18000000m, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4608), null, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4609), null },
                    { new Guid("7e9fe0da-2abe-4e58-bdfd-5d64a6549347"), new Guid("7e9fe0da-2abe-4e58-bdfd-5d64a6549d47"), 12000000m, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4615), null, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4615), null },
                    { new Guid("91c2f231-c3e9-4a13-a4d6-1ab2ca2c9765"), new Guid("91c2f231-c3e9-4a13-a4d6-1ab2ca2c9754"), 13000000m, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4617), null, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4621), null },
                    { new Guid("9dca19fd-072c-4d2f-b7a7-1d0d273f9034"), new Guid("9dca19fd-072c-4d2f-b7a7-1d0d273f9014"), 13000000m, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4625), null, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4626), null },
                    { new Guid("bd7ee2c3-5c10-4567-9a87-d071d6f8c312"), new Guid("bd7ee2c3-5c10-4567-9a87-d071d6f8c3b2"), 182000000m, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4612), null, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4612), null }
                });

            migrationBuilder.InsertData(
                table: "Courts",
                columns: new[] { "Id", "Address", "CourtAvatarImgUrls", "CourtName", "Created", "CreatedBy", "Description", "GoogleMapURLs", "ImageUrls", "IsDelete", "LastModified", "LastModifiedBy", "Latitude", "LatitudeDelta", "Longitude", "LongitudeDelta", "OwnerId", "PlaceId", "TimeEnd", "TimeStart", "WallpaperUrls" },
                values: new object[,]
                {
                    { new Guid("4f15e1fd-1f5c-40ef-9947-fa480a6859d1"), "26 Lê Quý Đôn, Phường Võ Thị Sáu, Quận 3, Thành phố Hồ Chí Minh", null, "Sân cầu lông Marie Curie", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4728), null, "Sân cầu lông hiện đại với sàn gỗ, thuận tiện cho các hoạt động thi đấu và tập luyện.", "https://maps.google.com/?q=26+Le+Quy+Don", "image1.jpge", false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4728), null, 10.7821421, null, 106.690265, null, new Guid("123e4567-e89b-12d3-a456-426614174001"), "10.786887, 106.690193", new TimeSpan(0, 22, 0, 0, 0), new TimeSpan(0, 5, 0, 0, 0), null },
                    { new Guid("58b1deaf-656b-4fe0-90d8-396c5479381f"), "36/5 Luy Bán Bích, Phường Tân Thới Hòa, Quận Tân Phú, Thành phố Hồ Chí Minh", null, "Sân bóng chuyền Tân Bình", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4731), null, "Sân bóng chuyền ngoài trời với không gian rộng rãi, phù hợp cho cả tập luyện và thi đấu.", "https://maps.google.com/?q=36/5+Luy+Bán+Bích", "image1.jpge", false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4731), null, 10.761793000000001, null, 106.632865, null, new Guid("123e4567-e89b-12d3-a456-426614174000"), "10.768199, 106.628938", new TimeSpan(0, 21, 0, 0, 0), new TimeSpan(0, 6, 0, 0, 0), null },
                    { new Guid("5ab1f835-cf9f-4847-b4a7-d0d20b183b44"), "Đ. Số 11/Hẻm 52 Tổ 1, Khu phố 9, Thủ Đức, Thành phố Hồ Chí Minh, Vietnam", null, "Sân bóng đá VNV", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4724), null, "Sân đẹp, cỏ xịn, đèn sáng, có chỗ để xe oto, bóng xịn", "https://maps.app.goo.gl/UUCSZm1p9ngEx7k79", "image1.jpge", false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4724), null, 10.844542499999999, null, 106.7526029, null, new Guid("123e4567-e89b-12d3-a456-426614174001"), "10.844905847478088, 106.75213708986735", new TimeSpan(0, 15, 30, 0, 0), new TimeSpan(0, 14, 30, 0, 0), null },
                    { new Guid("72f0c66d-700a-4c05-9f78-8b9fdd3a7cda"), "219 Lý Thường Kiệt, Phường 15, Quận 11, Thành phố Hồ Chí Minh", null, "Sân bóng đá Phú Thọ", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4736), null, "Sân bóng đá lớn với cỏ nhân tạo chất lượng cao, có khán đài và hệ thống chiếu sáng tốt.", "https://maps.google.com/?q=219+Lý+Thường+Kiệt", "image1.jpge", false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4736), null, 10.76754, null, 106.658356, null, new Guid("123e4567-e89b-12d3-a456-426614174001"), "10.769555, 106.663338", new TimeSpan(0, 23, 30, 0, 0), new TimeSpan(0, 4, 0, 0, 0), null },
                    { new Guid("d2642e7e-9a72-4e28-9c79-1e8e80134c8f"), "Số 45 Bùi Xương Trạch, phường Long Trường, Quận 9, Thành phố, Thủ Đức, Thành phố Hồ Chí Minh", null, "Sân bóng đá mini Long Trường Quận 9", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4715), null, "Sân bóng mini tiêu chuẩn cao (chuẩn FiFa) với hệ thống phụ trợ (nhà thay đồ, nhà tắm, nhà vệ sinh) sạch sẽ thoáng mát duy nhất.", "https://maps.app.goo.gl/s6yWXEpDYU1DNjuF6", "image1.jpge", false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4715), null, 10.8032638, null, 106.81126829999999, null, new Guid("123e4567-e89b-12d3-a456-426614174000"), "10.805515145695411, 106.81088572205702", new TimeSpan(0, 23, 59, 59, 0), new TimeSpan(0, 4, 0, 0, 0), null },
                    { new Guid("ef2bd841-3214-434b-95aa-080165f5a2b2"), "40 Đ. Số 11, Trường Thọ, Thủ Đức, Thành phố Hồ Chí Minh, Vietnam", null, "Sân cầu lông B-ZONE 11", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4719), null, "Sân cầu lông trang bị tiện nghi đầy đủ, giữ xe an ninh", "https://maps.app.goo.gl/cwrHGkHsM4769eSE7", "image1.jpge", false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4719), null, 10.8447102, null, 106.7530548, null, new Guid("123e4567-e89b-12d3-a456-426614174001"), "10.845057917596483, 106.75295823555061", new TimeSpan(0, 22, 0, 0, 0), new TimeSpan(0, 5, 0, 0, 0), null }
                });

            migrationBuilder.InsertData(
                table: "Campaigns",
                columns: new[] { "Id", "CampaignImageURL", "CampaignName", "CourtId", "Created", "CreatedBy", "Description", "EndDateApplying", "IsDelete", "LastModified", "LastModifiedBy", "MaxValueDiscount", "MinValueApply", "PercentDiscount", "QuantityOfCampaign", "ReasonOfReject", "SportTypeApply", "StartDateApplying", "Status" },
                values: new object[,]
                {
                    { new Guid("45a55f14-ac7d-4e58-b9a9-c830013d07f1"), "Campaign Image 4", "Birthday Discount!!!", new Guid("5ab1f835-cf9f-4847-b4a7-d0d20b183b44"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4918), null, null, new DateTime(2024, 7, 27, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4916), false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4918), null, 50000m, 1m, 25m, 10, null, "All", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4916), 1 },
                    { new Guid("7f34ee57-38bc-4852-a7d6-57f1b26ed5af"), "Campaign Image 2", "Christmas Discount!!!", new Guid("58b1deaf-656b-4fe0-90d8-396c5479381f"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4908), null, null, new DateTime(2024, 8, 5, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4906), false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4908), null, 45000m, 20000m, 25m, 10, null, "Soccer", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4906), 1 },
                    { new Guid("9de56f74-7834-4aeb-b774-e18abc1bcedd"), "Campaign Image 3", "Lunar New Year Discount!!!", new Guid("ef2bd841-3214-434b-95aa-080165f5a2b2"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4913), null, null, new DateTime(2024, 7, 31, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4911), false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4914), null, 50000m, 0m, 30m, 10, null, "Volleyball", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4911), 1 },
                    { new Guid("d81fe96c-b8f4-4f64-b4f8-1a3bc9f41425"), "Campaign Image 1", "Discount 20% base price", new Guid("ef2bd841-3214-434b-95aa-080165f5a2b2"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4903), null, null, new DateTime(2024, 7, 30, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4896), false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4903), null, 20000m, 40000m, 20m, 20, null, "Badminton", new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4896), 1 }
                });

            migrationBuilder.InsertData(
                table: "CourtSubdivisions",
                columns: new[] { "Id", "BasePrice", "CourtId", "CourtSubdivisionDescription", "CourtSubdivisionName", "CourtSubdivisionSettingId", "Created", "CreatedBy", "CreatedStatus", "IsActive", "IsDelete", "LastModified", "LastModifiedBy", "ReasonOfRejected" },
                values: new object[,]
                {
                    { new Guid("20f46754-d281-44c6-aa5c-d97ac4f3d8cb"), 60000m, new Guid("ef2bd841-3214-434b-95aa-080165f5a2b2"), "Sân trong nhà", "Sân cầu lông B-ZONE 11, phân cấp 1", new Guid("63998125-8cbd-41b7-9123-a6c7ca3ad63e"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4831), null, 1, true, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4831), null, null },
                    { new Guid("45ed7684-340d-414b-ae8c-fda358f62ac2"), 60000m, new Guid("58b1deaf-656b-4fe0-90d8-396c5479381f"), "Sân bóng chuyền xi măng", "Sân bóng chuyền Ellen, phân cấp 1", new Guid("0c9e0496-e891-468c-aca5-6c09c1a8f159"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4823), null, 1, true, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4823), null, null },
                    { new Guid("7a62ef5e-fc97-48d3-a0a2-e9e290665f8d"), 110000m, new Guid("ef2bd841-3214-434b-95aa-080165f5a2b2"), "Sân trong nhà", "Sân cầu lông B-ZONE 11, phân cấp 2", new Guid("63998125-8cbd-41b7-9123-a6c7ca3ad63e"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4834), null, 2, true, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4834), null, null },
                    { new Guid("b5a7f639-aaa7-412d-8bde-7767489e6839"), 60000m, new Guid("58b1deaf-656b-4fe0-90d8-396c5479381f"), "Sân bóng chuyền trong nhà", "Sân bóng chuyền Ellen, phân cấp 2", new Guid("effd5616-ad35-4204-8c5e-01ad289855e8"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4826), null, 0, true, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4827), null, null },
                    { new Guid("c7a8a07c-dd21-4323-bb8a-25c073fabcde"), 110000m, new Guid("d2642e7e-9a72-4e28-9c79-1e8e80134c8f"), "Sân bóng đá nhân tạo", "Sân 1", new Guid("a93c57bd-f6d5-414e-a4b2-5aa269729a43"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4839), null, 1, true, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4839), null, null },
                    { new Guid("d104a1db-67e3-4351-9b3c-037ec06c245e"), 190000m, new Guid("d2642e7e-9a72-4e28-9c79-1e8e80134c8f"), "Sân bóng đá nhân tạo", "Sân 2", new Guid("a93c57bd-f6d5-414e-a4b2-5aa269729a43"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4842), null, 0, true, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4843), null, null },
                    { new Guid("e72938fe-50a0-4b5e-a898-a5cbf5b2039c"), 110000m, new Guid("ef2bd841-3214-434b-95aa-080165f5a2b2"), "Sân ngoài trời", "Sân cầu lông B-ZONE 11, phân cấp 3", new Guid("9ce93f4d-b691-4622-95a5-3825916409f6"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4836), null, 0, true, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4837), null, null }
                });

            migrationBuilder.InsertData(
                table: "TimePeriods",
                columns: new[] { "Id", "CourtId", "Created", "CreatedBy", "Description", "EndTime", "IsDelete", "IsRefundDeposit", "LastModified", "LastModifiedBy", "PercentDeposit", "RateMultiplier", "StartTime" },
                values: new object[,]
                {
                    { new Guid("03ac6742-bc41-4ee1-8057-657f0a6c331c"), new Guid("4f15e1fd-1f5c-40ef-9947-fa480a6859d1"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5082), null, "Giờ Bình Thường", new TimeSpan(0, 23, 0, 0, 0), false, false, null, null, null, 1.0m, new TimeSpan(0, 20, 30, 0, 0) },
                    { new Guid("34fe77a7-485c-4fc4-b7c9-20f7332538f9"), new Guid("ef2bd841-3214-434b-95aa-080165f5a2b2"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5073), null, "Giờ Cao Điểm", new TimeSpan(0, 20, 0, 0, 0), false, false, null, null, null, 1.5m, new TimeSpan(0, 17, 0, 0, 0) },
                    { new Guid("88f7363a-5d6f-41f7-881c-34aa89f10eb2"), new Guid("5ab1f835-cf9f-4847-b4a7-d0d20b183b44"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5080), null, "Giờ Thấp Điểm", new TimeSpan(0, 16, 0, 0, 0), false, false, null, null, null, 0.8m, new TimeSpan(0, 10, 0, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookingDate", "BookingStatus", "CampaignId", "CourtSubdivisionId", "Created", "CreatedBy", "CustomerId", "EndTimePlaying", "IsDelete", "IsDeposit", "IsRoomBooking", "LastModified", "LastModifiedBy", "PlayingDate", "StartTimePlaying", "TotalAmount" },
                values: new object[,]
                {
                    { new Guid("0fa91b15-e147-4a4c-931b-5a1abc2efb93"), new DateTime(2024, 7, 26, 0, 0, 0, 0, DateTimeKind.Local), "Approved", new Guid("d81fe96c-b8f4-4f64-b4f8-1a3bc9f41425"), new Guid("20f46754-d281-44c6-aa5c-d97ac4f3d8cb"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4966), null, new Guid("123e4567-e89b-12d3-a456-426614174100"), new TimeSpan(0, 20, 0, 0, 0), false, true, false, null, null, new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 18, 0, 0, 0), 1000m },
                    { new Guid("22ae1f0b-3b4a-4c7b-947e-3612c4b6a8cd"), new DateTime(2024, 7, 26, 0, 0, 0, 0, DateTimeKind.Local), "Approved", new Guid("7f34ee57-38bc-4852-a7d6-57f1b26ed5af"), new Guid("7a62ef5e-fc97-48d3-a0a2-e9e290665f8d"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4993), null, new Guid("123e4567-e89b-12d3-a456-426614174101"), new TimeSpan(0, 19, 0, 0, 0), false, false, true, null, null, new DateTime(2024, 7, 28, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 17, 0, 0, 0), 1200m },
                    { new Guid("6c4099e7-0731-4f9d-90ee-fb7791040777"), new DateTime(2024, 7, 26, 0, 0, 0, 0, DateTimeKind.Local), "Cancel", new Guid("d81fe96c-b8f4-4f64-b4f8-1a3bc9f41425"), new Guid("d104a1db-67e3-4351-9b3c-037ec06c245e"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5005), null, new Guid("123e4567-e89b-12d3-a456-426614174100"), new TimeSpan(0, 22, 0, 0, 0), false, false, false, null, null, new DateTime(2024, 7, 25, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 20, 0, 0, 0), 1800m },
                    { new Guid("eadc2d2e-3ad3-4d6f-a4b1-55b6b233fe2e"), new DateTime(2024, 7, 26, 0, 0, 0, 0, DateTimeKind.Local), "Rejected", new Guid("d81fe96c-b8f4-4f64-b4f8-1a3bc9f41425"), new Guid("d104a1db-67e3-4351-9b3c-037ec06c245e"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5001), null, new Guid("123e4567-e89b-12d3-a456-426614174102"), new TimeSpan(0, 22, 0, 0, 0), false, true, false, null, null, new DateTime(2024, 7, 29, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 20, 0, 0, 0), 1800m },
                    { new Guid("fba3e7b2-981f-4038-a306-7432db3ef4c6"), new DateTime(2024, 7, 26, 0, 0, 0, 0, DateTimeKind.Local), "Finished", new Guid("9de56f74-7834-4aeb-b774-e18abc1bcedd"), new Guid("e72938fe-50a0-4b5e-a898-a5cbf5b2039c"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(4997), null, new Guid("123e4567-e89b-12d3-a456-426614174102"), new TimeSpan(0, 22, 0, 0, 0), false, true, false, null, null, new DateTime(2024, 7, 26, 0, 0, 0, 0, DateTimeKind.Local), new TimeSpan(0, 20, 0, 0, 0), 800m }
                });

            migrationBuilder.InsertData(
                table: "TimeChecking",
                columns: new[] { "Id", "CourtSubdivisionId", "Created", "CreatedBy", "DateBooking", "EndTime", "IsDelete", "IsLock", "LastModified", "LastModifiedBy", "StartTime" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("20f46754-d281-44c6-aa5c-d97ac4f3d8cb"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5117), null, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5123), new DateTime(2024, 7, 26, 18, 7, 54, 174, DateTimeKind.Utc).AddTicks(5119), false, false, null, null, new DateTime(2024, 7, 26, 16, 7, 54, 174, DateTimeKind.Utc).AddTicks(5119) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("7a62ef5e-fc97-48d3-a0a2-e9e290665f8d"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5125), null, new DateTime(2024, 7, 27, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5127), new DateTime(2024, 7, 26, 19, 7, 54, 174, DateTimeKind.Utc).AddTicks(5126), false, true, null, null, new DateTime(2024, 7, 26, 17, 7, 54, 174, DateTimeKind.Utc).AddTicks(5125) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("e72938fe-50a0-4b5e-a898-a5cbf5b2039c"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5128), null, new DateTime(2024, 7, 27, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5129), new DateTime(2024, 7, 27, 18, 7, 54, 174, DateTimeKind.Utc).AddTicks(5129), false, false, null, null, new DateTime(2024, 7, 27, 16, 7, 54, 174, DateTimeKind.Utc).AddTicks(5128) },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("45ed7684-340d-414b-ae8c-fda358f62ac2"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5131), null, new DateTime(2024, 7, 28, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5132), new DateTime(2024, 7, 28, 18, 7, 54, 174, DateTimeKind.Utc).AddTicks(5132), false, true, null, null, new DateTime(2024, 7, 28, 16, 7, 54, 174, DateTimeKind.Utc).AddTicks(5131) },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("b5a7f639-aaa7-412d-8bde-7767489e6839"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5133), null, new DateTime(2024, 7, 29, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5135), new DateTime(2024, 7, 29, 17, 7, 54, 174, DateTimeKind.Utc).AddTicks(5134), false, false, null, null, new DateTime(2024, 7, 29, 15, 7, 54, 174, DateTimeKind.Utc).AddTicks(5134) }
                });

            migrationBuilder.InsertData(
                table: "RoomMatches",
                columns: new[] { "Id", "BookingId", "Created", "CreatedBy", "EndTimeRoom", "IsDelete", "IsPrivate", "LastModified", "LastModifiedBy", "LevelId", "MaximumMember", "Note", "RoomName", "RuleRoom", "SportCategory", "StartTimeRoom" },
                values: new object[,]
                {
                    { new Guid("a1e3c431-4f5b-4ebc-b485-82f456d012c4"), new Guid("0fa91b15-e147-4a4c-931b-5a1abc2efb93"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5033), null, new DateTime(2024, 7, 27, 0, 2, 54, 174, DateTimeKind.Local).AddTicks(5031), false, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5033), null, new Guid("1a2b3c4d-5e6f-4a5b-8c2d-3e4f567a89b1"), 20, "Note Sample", null, "Rule Room Sample", 0, new DateTime(2024, 7, 26, 23, 2, 54, 174, DateTimeKind.Local).AddTicks(5030) },
                    { new Guid("c7605db8-d9ab-4ab8-a1c8-14d30f955707"), new Guid("fba3e7b2-981f-4038-a306-7432db3ef4c6"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5056), null, new DateTime(2024, 7, 27, 0, 2, 54, 174, DateTimeKind.Local).AddTicks(5055), false, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5056), null, new Guid("3c4d5e6f-7a8b-4a5b-1c2d-3e4f5a6b7c8d"), 20, "Note Sample", null, "Rule Room Sample", 0, new DateTime(2024, 7, 26, 23, 2, 54, 174, DateTimeKind.Local).AddTicks(5054) },
                    { new Guid("ecb739f6-55a2-4318-aa17-824ed2c50e88"), new Guid("22ae1f0b-3b4a-4c7b-947e-3612c4b6a8cd"), new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5051), null, new DateTime(2024, 7, 27, 1, 2, 54, 174, DateTimeKind.Local).AddTicks(5050), false, false, new DateTime(2024, 7, 26, 14, 7, 54, 174, DateTimeKind.Utc).AddTicks(5052), null, new Guid("2b3c4d5e-6f7a-4a5b-0d1e-2f3a4b5c6d7e"), 20, "Note Sample", null, "Rule Room Sample", 0, new DateTime(2024, 7, 26, 23, 37, 54, 174, DateTimeKind.Local).AddTicks(5049) }
                });

            migrationBuilder.InsertData(
                table: "RoomMembers",
                columns: new[] { "CustomerId", "RoomMatchId", "RoleInRoom" },
                values: new object[,]
                {
                    { new Guid("123e4567-e89b-12d3-a456-426614174100"), new Guid("a1e3c431-4f5b-4ebc-b485-82f456d012c4"), 0 },
                    { new Guid("123e4567-e89b-12d3-a456-426614174101"), new Guid("a1e3c431-4f5b-4ebc-b485-82f456d012c4"), 1 },
                    { new Guid("123e4567-e89b-12d3-a456-426614174102"), new Guid("a1e3c431-4f5b-4ebc-b485-82f456d012c4"), 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CampaignId",
                table: "Bookings",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CourtSubdivisionId",
                table: "Bookings",
                column: "CourtSubdivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CustomerId",
                table: "Bookings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CourtId",
                table: "Campaigns",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_Courts_OwnerId",
                table: "Courts",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Courts_PlaceId",
                table: "Courts",
                column: "PlaceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourtSubdivisions_CourtId",
                table: "CourtSubdivisions",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtSubdivisions_CourtSubdivisionSettingId",
                table: "CourtSubdivisions",
                column: "CourtSubdivisionSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtSubdivisionSettings_SportCategoryId",
                table: "CourtSubdivisionSettings",
                column: "SportCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountId",
                table: "Customers",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_AccountId",
                table: "DeviceTokens",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_BookingId",
                table: "Feedbacks",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_CourtId",
                table: "Feedbacks",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserLogin<string>_UserId",
                table: "IdentityUserLogin<string>",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AccountId",
                table: "Notification",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Owners_AccountId",
                table: "Owners",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentNotifications_MerchantId",
                table: "PaymentNotifications",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentNotifications_PaymentId",
                table: "PaymentNotifications",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AccountId",
                table: "Payments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_MerchantId",
                table: "Payments",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentDestinationId",
                table: "Payments",
                column: "PaymentDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSignatures_PaymentId",
                table: "PaymentSignatures",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PaymentId",
                table: "PaymentTransactions",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_AccountId",
                table: "RefreshToken",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomMatches_BookingId",
                table: "RoomMatches",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomMatches_LevelId",
                table: "RoomMatches",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomMembers_RoomMatchId",
                table: "RoomMembers",
                column: "RoomMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomRequests_CustomerId",
                table: "RoomRequests",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomRequests_RoomMatchId",
                table: "RoomRequests",
                column: "RoomMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeChecking_CourtSubdivisionId",
                table: "TimeChecking",
                column: "CourtSubdivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_TimePeriods_CourtId",
                table: "TimePeriods",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WalletId",
                table: "Transactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_AccountId",
                table: "Wallets",
                column: "AccountId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "DeviceFlowCodes");

            migrationBuilder.DropTable(
                name: "DeviceTokens");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "IdentityUserLogin<string>");

            migrationBuilder.DropTable(
                name: "IdentityUserRole<string>");

            migrationBuilder.DropTable(
                name: "IdentityUserToken<string>");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PaymentNotifications");

            migrationBuilder.DropTable(
                name: "PaymentSignatures");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "PersistedGrant");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "RoomMembers");

            migrationBuilder.DropTable(
                name: "RoomRequests");

            migrationBuilder.DropTable(
                name: "TimeChecking");

            migrationBuilder.DropTable(
                name: "TimePeriods");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "IdentityUser");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "RoomMatches");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Merchants");

            migrationBuilder.DropTable(
                name: "PaymentsDestinations");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Levels");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "CourtSubdivisions");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "CourtSubdivisionSettings");

            migrationBuilder.DropTable(
                name: "Courts");

            migrationBuilder.DropTable(
                name: "SportsCategories");

            migrationBuilder.DropTable(
                name: "Owners");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
