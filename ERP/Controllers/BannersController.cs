using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Catalog_Service.Entities;
using Catalog_Service.Features.BannersFeature.CreateBanner;
using Catalog_Service.Features.BannersFeature.DeleteBanner;
using Catalog_Service.Features.BannersFeature.GetActiveBanners;
using Catalog_Service.Features.BannersFeature.GetAllBanners;
using Catalog_Service.Features.BannersFeature.UpdateBanner;

namespace Catalog_Service.Controllers
{
    [ApiController]
    [Authorize(Policy = "AdminPolicy")]
    [Route("api/[controller]")]
    public class BannersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BannersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBannerCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Created($"/api/banners/{result.Data?.Id}", result)
                : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllBannersQuery());
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive([FromQuery] BannerPosition? position)
        {
            var result = await _mediator.Send(new GetActiveBannersQuery(position));
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBannerRequest request)
        {
            var command = new UpdateBannerCommand(
                id,
                request.Title,
                request.TitleAr,
                request.Subtitle,
                request.SubtitleAr,
                request.DesktopImageUrl,
                request.MobileImageUrl,
                request.CtaText,
                request.CtaTextAr,
                request.CtaLink,
                request.Position,
                request.SortOrder,
                request.ValidFrom,
                request.ValidUntil,
                request.IsActive);

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteBannerCommand(id));
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }
    }
}
