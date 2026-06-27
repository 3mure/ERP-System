using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Inventory.TransferStock
{
    [ApiController]
    [Route("api/v1/inventory")]
    public class TransferStockController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TransferStockController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// POST /api/v1/inventory/transfer
        ///
        /// Moves stock from one warehouse to another atomically.
        /// - Validates sufficient available stock before processing.
        /// - Returns a Manifest object with all transfer details
        ///   for the driver's printable document.
        /// </summary>
        [HttpPost("transfer")]
        [ProducesResponseType(typeof(TransferStockResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TransferStock([FromBody] TransferStockRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(new TransferStockCommand(request));

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result);
        }
    }
}
