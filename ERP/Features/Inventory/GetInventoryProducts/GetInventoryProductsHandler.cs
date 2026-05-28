using BuildingBlocks.Interfaces;
using Catalog_Service.Features.Customers.GetCustomers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Inventory.GetInventoryProducts
{
    public class GetInventoryProductsHandler
     : IRequestHandler<GetInventoryProductsQuery, GetInventoryProductsResult>
    {
        private readonly IBaseRepository<Entities.Inventory> _inventoryRepo;
        private const int PageSize = 10;

        public GetInventoryProductsHandler(IBaseRepository<Entities.Inventory> inventoryRepo)
            => _inventoryRepo = inventoryRepo;

        public async Task<GetInventoryProductsResult> Handle(
            GetInventoryProductsQuery query,
            CancellationToken cancellationToken)
        {
            var baseQuery = InventoryProductsFilter.Apply(
                _inventoryRepo.Get(i => !i.IsDeleted && i.Product.IsActive),
                query.Search,
                query.WarehouseId,
                query.Status);

            var totalCount = await baseQuery.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)totalCount / PageSize);
            var page = Math.Max(1, query.Page);

            var rows = await baseQuery
                .OrderBy(i => i.Product.Name)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Include(i => i.Product).ThenInclude(p => p.Category)
                .Include(i => i.Warehouse)
                .ToListAsync(cancellationToken);

            return new GetInventoryProductsResult(
                Data: rows.Select(InventoryProductsFilter.ToDto).ToList(),
                Pagination: new PaginationDto(page, totalPages));
        }
    }

}
