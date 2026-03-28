using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelDescriptionsApi.Data;
using ModelDescriptionsApi.Models;
using ModelDescriptionsApi.Models.DTO;

namespace ModelDescriptionsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreateAsset : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CreateAsset(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Asset3dDTO>> Asset3dDTO([FromBody] Asset3dDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("No availabe name");
            }
            if (string.IsNullOrWhiteSpace(dto.ModelPath))
            {
                return BadRequest("No available filepath");
            }

            //checking for duplicates
            var hasDuplicates = await _context.asset3ds.AnyAsync(a =>
                a.Name == dto.Name || a.ModelPath == dto.ModelPath
            );
            if (hasDuplicates)
                return Conflict(
                    $"An asset  with the same name {dto.Name} already exists or same file path {dto.ModelPath}, please change."
                );

            var asset3d = new Asset3d
            {
                Name = dto.Name,
                Description = dto.Description,
                ModelPath = dto.ModelPath,
            };

            //save in the ApplicationDbContext
            _context.asset3ds.Add(asset3d);
            await _context.SaveChangesAsync();

            var resultDto = new Asset3dDTO
            {
                Name = asset3d.Name,
                Description = asset3d.Description,
                ModelPath = asset3d.ModelPath,
            };

            return CreatedAtAction(nameof(GetAsset), new { id = asset3d.Id }, resultDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Asset3dDTO>> GetAsset(int id)
        {
            var asset = await _context.asset3ds.FindAsync(id);
            if (asset == null)
                return NotFound();
            return new Asset3dDTO
            {
                Name = asset.Name,
                Description = asset.Description,
                ModelPath = asset.ModelPath,
            };
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Asset3dDTO>> EditAsset(int id, [FromBody] Asset3dDTO dto)
        {
            if (dto == null)
                return BadRequest("Your change is empty");

            var existingAsset = await _context.asset3ds.FindAsync(id);
            if (existingAsset == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is requierd");
            if (string.IsNullOrWhiteSpace(dto.ModelPath))
                return BadRequest("ModelPath is requierd");

            bool duplicatesExist = await _context.asset3ds.AnyAsync(a =>
                a.Id != id && (a.Name == dto.Name || a.ModelPath == dto.ModelPath)
            );
            if (duplicatesExist)
                return Conflict("Another asset already has this Name or ModelPath");

            existingAsset.Name = dto.Name;
            existingAsset.Description = dto.Description;
            existingAsset.ModelPath = dto.ModelPath;

            await _context.SaveChangesAsync();

            var resultDto = new Asset3dDTO
            {
                Name = existingAsset.Name,
                Description = existingAsset.Description,
                ModelPath = existingAsset.ModelPath,
            };

            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Asset3dDTO>> DeleteAsset(int id)
        {
            var findAsset = await _context.asset3ds.FindAsync(id);

            if (findAsset == null)
                return NotFound();

            _context.asset3ds.Remove(findAsset);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Pong");
        }

        [HttpGet("byname/{name}")]
        public async Task<ActionResult<Asset3dDTO>> GetAssetByName(string name)
        {
            var metaData = await _context.asset3ds.FirstOrDefaultAsync(a => a.Name == name);
            if (metaData == null)
                return NotFound($"No such modelName: {name}");

            return new Asset3dDTO
            {
                Name = metaData.Name,
                Description = metaData.Description,
                ModelPath = metaData.ModelPath,
            };
        }
    }
}
