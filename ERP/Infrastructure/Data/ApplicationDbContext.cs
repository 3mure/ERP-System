using BuildingBlocks.SharedEntities;
using Catalog_Service.Entities;
using Catalog_Service.Entities.Retail;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog_Service.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // ===================== CATALOG =====================
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Occasion> Occasions { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }
        public DbSet<StockAlert> StockAlerts { get; set; }
        public DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public DbSet<ProductOccasion> ProductOccasions { get; set; }
        public DbSet<Banner> Banners { get; set; }

        // ===================== ERP =====================
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryAdjustment> InventoryAdjustments { get; set; }
        public DbSet<StockTransfer> StockTransfers { get; set; }
        public DbSet<StockTransferItem> StockTransferItems { get; set; }
        public DbSet<CustomerGroup> CustomerGroups { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<DemandLog> DemandLogs { get; set; }
        public DbSet<PosShift> PosShifts { get; set; }
        public DbSet<FinanceEntry> FinanceEntries { get; set; }
        public DbSet<InvoiceAuditLog> InvoiceAuditLogs { get; set; }
        public DbSet<AlertThreshold> AlertThresholds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===================== MASSTRANSIT =====================
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

            // ===================== CATALOG =====================

            modelBuilder.Entity<ProductOccasion>()
                .HasKey(po => new { po.ProductId, po.OccasionId });

            modelBuilder.Entity<ProductOccasion>()
                .HasOne(po => po.Product)
                .WithMany(p => p.ProductOccasions)
                .HasForeignKey(po => po.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProductOccasion>()
                .HasOne(po => po.Occasion)
                .WithMany(o => o.ProductOccasions)
                .HasForeignKey(po => po.OccasionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProductSpecification>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.Specifications)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction); // ✅ FIX

                entity.HasOne(p => p.Supplier)
                    .WithMany(s => s.Products)
                    .HasForeignKey(p => p.SupplierId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.Property(p => p.CostPrice).HasPrecision(12, 2);

                entity.HasIndex(p => p.ProductCode)
                    .IsUnique()
                    .HasFilter("[ProductCode] IS NOT NULL");
            });

            // ===================== ERP =====================

            ConfigureErp(modelBuilder);

            // ===================== GLOBAL FILTER =====================

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(CreateIsDeletedFilter(entityType.ClrType));
                }
            }
        }

        private void ConfigureErp(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockTransfer>(entity =>
            {
                entity.HasOne(t => t.FromWarehouse)
                    .WithMany(w => w.OutgoingTransfers)
                    .HasForeignKey(t => t.FromWarehouseId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(t => t.ToWarehouse)
                    .WithMany(w => w.IncomingTransfers)
                    .HasForeignKey(t => t.ToWarehouseId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(t => t.AuthorizedByUser)
                    .WithMany(u => u.AuthorizedTransfers)
                    .HasForeignKey(t => t.AuthorizedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(t => t.TransferReference).IsUnique();
            });

            modelBuilder.Entity<SalesOrder>(entity =>
            {
                entity.HasOne(o => o.Cashier).WithMany().HasForeignKey(o => o.CashierId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(o => o.Customer).WithMany().HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(o => o.Branch).WithMany().HasForeignKey(o => o.BranchId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(o => o.PaymentMethod).WithMany().HasForeignKey(o => o.PaymentMethodId).OnDelete(DeleteBehavior.NoAction);
                entity.Property(o => o.TotalAmount).HasPrecision(12, 2);
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(i => i.Warehouse).WithMany(w => w.Inventories).HasForeignKey(i => i.WarehouseId).OnDelete(DeleteBehavior.NoAction);
                entity.HasIndex(i => new { i.ProductId, i.WarehouseId }).IsUnique();
            });
        }

        private static LambdaExpression CreateIsDeletedFilter(Type type)
        {
            var param = Expression.Parameter(type, "e");
            var prop = Expression.Property(param, "IsDeleted");
            var body = Expression.Equal(prop, Expression.Constant(false));
            return Expression.Lambda(body, param);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseEntity entity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedAt = DateTime.UtcNow;
                        entity.IsDeleted = false;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entity.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        entry.State = EntityState.Modified;
                        entity.IsDeleted = true;
                        entity.DeletedAt = DateTime.UtcNow;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}