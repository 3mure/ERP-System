using Catalog_Service.Entities;
using BuildingBlocks.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalog_Service.Features.Shared;

namespace Catalog_Service.Features.ProductsFeature.StockManagement
{
    // --- Commands ---

    public record AddStockCommand(int ProductId, int QuantityToAdd) : IRequest<EndpointResponse<int>>;
    public record SetStockCommand(int ProductId, int NewQuantity) : IRequest<EndpointResponse<int>>;
    public record UpdateStockSettingsCommand(int ProductId, int MinStock, int MaxStock) : IRequest<EndpointResponse<bool>>;

    // --- Validators ---

    public class AddStockValidator : AbstractValidator<AddStockCommand>
    {
        public AddStockValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.QuantityToAdd).GreaterThan(0);
        }
    }

    public class SetStockValidator : AbstractValidator<SetStockCommand>
    {
        public SetStockValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.NewQuantity).GreaterThanOrEqualTo(0);
        }
    }

    public class UpdateStockSettingsValidator : AbstractValidator<UpdateStockSettingsCommand>
    {
        public UpdateStockSettingsValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.MinStock).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxStock).GreaterThan(x => x.MinStock).WithMessage("MaxStock must be greater than MinStock");
        }
    }

    // --- Handler ---

    public class StockManagementHandler : 
        IRequestHandler<AddStockCommand, EndpointResponse<int>>,
        IRequestHandler<SetStockCommand, EndpointResponse<int>>,
        IRequestHandler<UpdateStockSettingsCommand, EndpointResponse<bool>>
    {
        private readonly IBaseRepository<Product> _productRepo;
        private readonly IUnitOfWork _unitOfWork;

        public StockManagementHandler(IBaseRepository<Product> productRepo, IUnitOfWork unitOfWork)
        {
            _productRepo = productRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<EndpointResponse<int>> Handle(AddStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByIdAsync(request.ProductId);
            if (product == null) return EndpointResponse<int>.ErrorResponse("Product not found", 404);

            if (product.MaxStock > 0 && (product.StockQuantity + request.QuantityToAdd) > product.MaxStock)
            {
                return EndpointResponse<int>.ErrorResponse($"Cannot add stock. Total would exceed MaxStock of {product.MaxStock}", 400);
            }

            product.StockQuantity += request.QuantityToAdd;
            if (product.StockQuantity > 0) product.IsAvailable = true; // Auto-activate if stock added? Optional logic.

            _productRepo.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return EndpointResponse<int>.SuccessResponse(product.StockQuantity, "Stock added successfully");
        }

        public async Task<EndpointResponse<int>> Handle(SetStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByIdAsync(request.ProductId);
            if (product == null) return EndpointResponse<int>.ErrorResponse("Product not found", 404);

            if (product.MaxStock > 0 && request.NewQuantity > product.MaxStock)
            {
                 return EndpointResponse<int>.ErrorResponse($"Cannot set stock. New quantity exceeds MaxStock of {product.MaxStock}", 400);
            }

            product.StockQuantity = request.NewQuantity;
            
            // Logic: if setting to > 0, ensure Available? 
            if (product.StockQuantity > 0) product.IsAvailable = true;

            _productRepo.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return EndpointResponse<int>.SuccessResponse(product.StockQuantity, "Stock updated successfully");
        }

        public async Task<EndpointResponse<bool>> Handle(UpdateStockSettingsCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByIdAsync(request.ProductId);
            if (product == null) return EndpointResponse<bool>.ErrorResponse("Product not found", 404);

            product.MinStock = request.MinStock;
            product.MaxStock = request.MaxStock;

            _productRepo.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return EndpointResponse<bool>.SuccessResponse(true, "Stock settings updated successfully");
        }
    }
}
