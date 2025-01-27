using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sport.API.Entities;

namespace Sport.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompetitionsController : Controller
{
    private readonly SportDbContext _context;

    public CompetitionsController(SportDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompetitions()
    {
        var competitions = await _context.Competitions.ToListAsync();
        return Ok(competitions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompetition(int id)
    {
        var competition = await _context.Competitions.FindAsync(id);
        if (competition == null)
        {
            return NotFound();
        }
        
        return Ok(competition);
    }

    [HttpPost]
    public async Task<IActionResult> PostCompetition(Competition competition)
    {
        await _context.Competitions.AddAsync(competition);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetCompetition), new { id = competition.Id }, competition);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCompetition(int id, Competition competition)
    {
        if (id != competition.Id)
        {
            return BadRequest();
        }
        if (!_context.Competitions.Any(c => c.Id == id))
        {
            return NotFound();
        }
        
        _context.Entry(competition).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompetition(int id)
    {
        var competition = await _context.Competitions.FindAsync(id);
        if (competition == null)
        {
            return NotFound();
        }
        
        _context.Competitions.Remove(competition);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}