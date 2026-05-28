using MediatR;

namespace Catalog_Service.Features.Inventory.TransferStock
{
    public record TransferStockCommand(TransferStockRequest Request)
     : IRequest<TransferStockResult>;
}
