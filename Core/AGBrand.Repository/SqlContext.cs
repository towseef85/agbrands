using AGBrand.Models;
using AGBrand.Models.Domain;
using AGBrand.Packages.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AGBrand.Repository
{
    public class SqlContext : DbContext//, IRepository
    {
        ////private readonly IAuthService _authService;
        private readonly IContextLogger _logger;

        ////private readonly RoleCategory _roleCategory;
        ////private readonly RoleType _roleType;

        ////public readonly string Schema;

        public SqlContext(DbContextOptions options, IContextLogger logger/*, IAuthService authService*/) : base(options)
        {
            _logger = logger;
            ////_authService = authService;
            ////_roleCategory = authService.Auth.RoleCategory;
            ////_roleType = authService.Auth.RoleType;
            ////Schema = $"{authService.Auth.RoleCategory}_{authService.Auth.RoleType}";
        }

        public DbSet<Models.Domain.Action> Actions { get; set; }
        public DbSet<Otp> Otps { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSpec> ProductSpecs { get; set; }
        public DbSet<ProductSpecAttribute> ProductSpecAttributes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Variant> Variants { get; set; }
        public DbSet<VariantValue> VariantValues { get; set; }
        public DbSet<ProductVariantValue> ProductVariantValues { get; set; }
        public DbSet<CategoryProductCache> CategoryProductsCache { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        public override int SaveChanges()
        {
            AddAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        ////public Task<int> SaveChangesAsync()
        ////{
        ////    AddAuditInfo();
        ////    return base.SaveChangesAsync();
        ////}

        ////protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        ////{
        ////    base.OnConfiguring(optionsBuilder);

        ////    //optionsBuilder.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
        ////}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            BuildTables(modelBuilder);

            //Building Views
            BuildQueries(modelBuilder);

            //Has Enum Conversion
            BuildEnumConversion(modelBuilder);

            //Has Composite Keys
            BuildCompositeKeys(modelBuilder);

            // Creating Index
            BuildIndex(modelBuilder);

            //Has Query Filter
            BuildFilters(modelBuilder);
        }

        private static void BuildCompositeKeys(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>().HasKey(c => new { c.Id, c.ProductId });//Composite primary Key
            modelBuilder.Entity<CategoryProductCache>().HasKey(c => new { c.CategoryId, c.ProductId });

            modelBuilder.Entity<CategoryProductCache>().HasOne(c => c.Product).WithMany(c => c.AssociatedCategoriesCache).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<CategoryProductCache>().HasOne(c => c.Category).WithMany(c => c.AssociatedProductsCache).OnDelete(DeleteBehavior.NoAction);
        }

        private static void BuildEnumConversion(ModelBuilder modelBuilder)
        {
            var discountTypeConverter = new ValueConverter<DiscountType, string>(
                v => v.ToString(),
                v => (DiscountType)Enum.Parse(typeof(DiscountType), v));

            modelBuilder.Entity<Product>().Property(c => c.DiscountType).HasConversion(discountTypeConverter);
        }

        private static void BuildIndex(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(c => new { c.Mobile, c.Email });
        }

        private void AddAuditInfo()
        {
            var timestamp = DateTime.UtcNow;
            ////var jsonSettings = new JsonSerializerSettings
            ////{
            ////    DefaultValueHandling = DefaultValueHandling.Ignore,
            ////    NullValueHandling = NullValueHandling.Ignore,
            ////    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            ////};

            foreach (var entry in ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified)))
            {
                ////var serializedEntity = JsonConvert.SerializeObject(entry.Entity, Formatting.Indented, jsonSettings);

                if (entry.State == EntityState.Added)
                {
                    ////_logger.Log($"Record Added By: ({_authService.Auth?.AuthId}) on {timestamp}:=> {serializedEntity}");
                    ((BaseEntity)entry.Entity).CreatedOn = timestamp;
                }

                ////_logger.Log($"Record Updated By: ({_authService.Auth?.AuthId}) on {timestamp}:=> {serializedEntity}");
                ((BaseEntity)entry.Entity).UpdatedOn = timestamp;
            }
        }

        private void BuildFilters(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(p => !p.IsDeleted);// With IsDeleted Filters

            modelBuilder.Entity<Category>().HasQueryFilter(p => !p.IsDeleted); // With IsDeleted Filters
            modelBuilder.Entity<Brand>().HasQueryFilter(p => !p.IsDeleted); // With IsDeleted Filters



            ////switch (_roleCategory)
            ////{
            ////    case RoleCategory.System:
            ////        {
            ////            modelBuilder.Entity<Customer>().HasQueryFilter(p => !p.IsDeleted); // With IsDeleted Filters

            ////            break;
            ////        }

            ////    case RoleCategory.Manufacturer:
            ////        {
            ////            modelBuilder.Entity<Customer>().HasQueryFilter(p =>
            ////                !p.IsDeleted &&
            ////                p.ManufacturerId == _authService.Auth.ContextManufacturerId); // With IsDeleted Filters
            ////
            ////            switch (_roleType)
            ////            {
            ////                case RoleType.OemTeamUser:
            ////                    modelBuilder.Entity<UserDevice>()
            ////                        .HasQueryFilter(p =>
            ////                            p.UserId == _authService.Auth.ContextUserId); // With IsDeleted Filters
            ////                    break;
            ////            }

            ////            break;
            ////        }

            ////    case RoleCategory.Customer:
            ////        {
            ////            modelBuilder.Entity<Customer>()
            ////                .HasQueryFilter(p =>
            ////                    !p.IsDeleted && p.Id == _authService.Auth.ContextCustomerId); // With IsDeleted Filters
            ////
            ////            switch (_roleType)
            ////            {
            ////                case RoleType.CustomerTeamUser:
            ////                    modelBuilder.Entity<UserDevice>()
            ////                        .HasQueryFilter(p =>
            ////                            p.UserId == _authService.Auth.ContextUserId); // With IsDeleted Filters
            ////                    break;
            ////            }

            ////            break;
            ////        }

            ////    default:
            ////        break;
            ////}
        }

        private void BuildQueries(ModelBuilder modelBuilder)
        {
            ////modelBuilder.Query<ViewDevice>()
            ////    .ToQuery(() => Devices.Where(c => !c.IsDeleted).Select(c => new ViewDevice
            ////    {
            ////        SigfoxId = c.SigfoxId,
            ////        Id = c.Id,
            ////        IsDeleted = c.IsDeleted
            ////    }));
        }

        private void BuildTables(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable(nameof(Users));
            modelBuilder.Entity<Otp>().ToTable(nameof(Otps));
            modelBuilder.Entity<RefreshToken>().ToTable(nameof(RefreshTokens));
            modelBuilder.Entity<Models.Domain.Action>().ToTable(nameof(Actions));
            modelBuilder.Entity<Product>().ToTable(nameof(Products));
            modelBuilder.Entity<ProductSpec>().ToTable(nameof(ProductSpecs));
            modelBuilder.Entity<ProductSpecAttribute>().ToTable(nameof(ProductSpecAttributes));
            modelBuilder.Entity<Tag>().ToTable(nameof(Tags));
            modelBuilder.Entity<Brand>().ToTable(nameof(Brands));
            modelBuilder.Entity<Category>().ToTable(nameof(Categories));
            modelBuilder.Entity<Variant>().ToTable(nameof(Variants));
            modelBuilder.Entity<VariantValue>().ToTable(nameof(VariantValues));
            modelBuilder.Entity<ProductVariantValue>().ToTable(nameof(ProductVariantValues));
            modelBuilder.Entity<CategoryProductCache>().ToTable(nameof(CategoryProductsCache));
            modelBuilder.Entity<ProductImage>().ToTable(nameof(ProductImages));
        }
    }
}
