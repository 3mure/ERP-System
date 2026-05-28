using Catalog_Service.Entities.Retail;
using Catalog_Service.Features.RetailFeature.Roles;
using Catalog_Service.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Controllers
{
    [ApiController]
    [Route("api/retail/roles")]
    public class RetailRolesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public RetailRolesController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> List(CancellationToken ct)
        {
            var list = await _db.Roles .AsNoTracking().OrderBy(r => r.Name).ToListAsync(ct);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var role = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, ct);
            return role is null ? NotFound() : Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleDto dto, CancellationToken ct)
        {
            var entity = new Entities.Role { Name = dto.Name.Trim() };
            _db.Roles.Add(entity);
            await _db.SaveChangesAsync(ct);
            return Created($"/api/retail/roles/{entity.Id}", entity);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto dto, CancellationToken ct)
        {
            var entity = await _db.Roles.FirstOrDefaultAsync(r => r.Id == id, ct);
            if (entity is null) return NotFound();
            entity.Name = dto.Name.Trim();
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var entity = await _db.Roles.FirstOrDefaultAsync(r => r.Id == id, ct);
            if (entity is null) return NotFound();
            _db.Roles.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}
