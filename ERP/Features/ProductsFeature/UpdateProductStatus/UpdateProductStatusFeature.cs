using Catalog_Service.Entities;
using BuildingBlocks.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalog_Service.Features.Shared;

namespace Catalog_Service.Features.ProductsFeature.UpdateProductStatus
{
    // --- Command ---
    public record UpdateProductStatusCommand(int Id, bool IsAvailable) : IRequest<EndpointResponse<bool>>;

    // --- Handler ---
    public class UpdateProductStatusHandler : IRequestHandler<UpdateProductStatusCommand, EndpointResponse<bool>>
    {
        private readonly IBaseRepository<Product> _productRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductStatusHandler(IBaseRepository<Product> productRepo, IUnitOfWork unitOfWork)
        {
            _productRepo = productRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<EndpointResponse<bool>> Handle(UpdateProductStatusCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByIdAsync(request.Id);
            if (product == null)
                return EndpointResponse<bool>.ErrorResponse("Product not found", 404);

            product.IsAvailable = request.IsAvailable;
            
            _productRepo.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return EndpointResponse<bool>.SuccessResponse(true, $"Product status updated to {(request.IsAvailable ? "Active" : "Inactive")}");
        }
    }
}
