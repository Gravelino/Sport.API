using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sport.API.Entities;

namespace Sport.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParticipantCompetitionsController : Controller
{
    private readonly SportDbContext _context;

    public ParticipantCompetitionsController(SportDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetParticipantCompetitions()
    {
        var result = await _context.ParticipantCompetitions.ToListAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetParticipantCompetition(int id)
    {
        var result = await _context.ParticipantCompetitions.FindAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        
        return Ok(result);
    }

    [HttpGet("get-by-participantId/{participantId}")]
    public async Task<IActionResult> GetParticipantCompetitionsByParticipantId(int participantId)
    {
        var result = await _context.ParticipantCompetitions.Where(p => p.ParticipantId == participantId).ToListAsync();
        if (result == null)
        {
            return NotFound();
        }
        
        return Ok(result);
    }

    [HttpGet("get-by-competitionId/{competitionId}")]
    public async Task<IActionResult> GetParticipantCompetitionsByCompetitionId(int competitionId)
    {
        var result = await _context.ParticipantCompetitions.Where(c => c.CompetitionId == competitionId).ToListAsync();
        if (result == null)
        {
            return NotFound();
        }
        
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostParticipantCompetition(ParticipantCompetition participantCompetition)
    {
        await _context.ParticipantCompetitions.AddAsync(participantCompetition);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetParticipantCompetition), new { id = participantCompetition.ParticipantId }, participantCompetition);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutParticipantCompetition(int id, ParticipantCompetition participantCompetition)
    {
        if(id != participantCompetition.Id)
        {
            return BadRequest();
        }
        if (!_context.ParticipantCompetitions.Any(e => e.Id == participantCompetition.Id))
        {
            return NotFound();
        }
        
        _context.Entry(participantCompetition).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteParticipantCompetition(int id)
    {
        var participantCompetition = await _context.ParticipantCompetitions.FindAsync(id);
        if (participantCompetition == null)
        {
            return NotFound();
        }
        
        _context.ParticipantCompetitions.Remove(participantCompetition);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}