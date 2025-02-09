using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sport.API.Entities;

namespace Sport.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipantsController : Controller
{
    private readonly SportDbContext _context;

    public ParticipantsController(SportDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllParticipants()
    {
        var participants = await _context.Participants.ToListAsync();
        return Ok(participants);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetParticipantById(int id)
    {
        var participant = await _context.Participants.FindAsync(id);
        if (participant == null)
        {
            return NotFound();
        }

        return Ok(participant);
    }

    [HttpPost]
    public async Task<IActionResult> AddParticipant(Participant participant)
    {
        await _context.Participants.AddAsync(participant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetParticipantById), new { id = participant.Id }, participant);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateParticipant(int id, Participant participant)
    {
        if (id != participant.Id)
        {
            return BadRequest();
        }

        if (!_context.Participants.Any(c => c.Id == id))
        {
            return NotFound();
        }

        _context.Entry(participant).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteParticipant(int id)
    {
        var participant = await _context.Participants.FindAsync(id);
        if (participant == null)
        {
            return NotFound();
        }

        _context.Participants.Remove(participant);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{participantId}/scores")]
    public async Task<IActionResult> GetParticipantScores(int participantId)
    {
        var participant = await _context.Participants.FindAsync(participantId);
        if (participant == null)
        {
            return NotFound("Participant not found");
        }
        
        var scores = await _context.ParticipantCompetitions
            .Where(pc => pc.ParticipantId == participantId)
            .Include(pc => pc.Competition)
            .ToListAsync();
        
        return Ok(scores);
    }
}