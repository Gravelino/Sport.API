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

    [HttpGet("get-by-date/{date}")]
    public async Task<IActionResult> GetCompetitionsByDate(string date)
    {
        var competitions = await _context.Competitions.Where(c => c.StartDate == DateTime.Parse(date)).ToListAsync();
        if (competitions.Count == 0)
        {
            return NotFound();
        }
        
        return Ok(competitions);
    }

    [HttpGet("get-by-location/{location}")]
    public async Task<IActionResult> GetCompetitionsByLocation(string location)
    {
        var competitions = await _context.Competitions.Where(c => c.Location == location).ToListAsync();
        if (competitions.Count == 0)
        {
            return NotFound();
        }
        
        return Ok(competitions);
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

    [HttpGet("{competitionId}/ranking")]
    public async Task<IActionResult> GetRanking(int competitionId)
    {
        var competition = await _context.Competitions.FindAsync(competitionId);
        if (competition == null)
        {
            return NotFound("Competition not found");
        }
        
        var ranking = await _context.ParticipantCompetitions
            .Where(pc => pc.CompetitionId == competitionId)
            .OrderByDescending(pc => pc.ParticipantScore)
            .Include(pc => pc.Participant)
            .ToListAsync();
        
        return Ok(ranking);
    }
    
    [HttpGet("global-ranking")]
    public async Task<IActionResult> GetGlobalRanking()
    {
        var ranking = await _context.Participants
            .OrderByDescending(p => p.TotalScore)
            .ToListAsync();
        
        return Ok(ranking);
    }
    
    [HttpPut("{competitionId}/end")]
    public async Task<IActionResult> EndCompetition(int competitionId)
    {
        var competition = await _context.Competitions
            .Include(c => c.Participants)
            .Where(c => c.Id == competitionId)
            .FirstAsync();
        if (competition == null)
        {
            return NotFound("Competition not found.");
        }
        
        if (competition.EndDate is not null)
        {
            return BadRequest($"Competition has already ended on {competition.EndDate}");
        }
        if (competition.StartDate > DateTime.Now)
        {
            return BadRequest($"Competition has not started yet. Starts on {competition.StartDate}");
        }

        if (competition.Participants.Count == 0)
        {
            return BadRequest($"There are no participants on this competition.");
        }
        
        var participantCompetitions = await _context.ParticipantCompetitions
            .Where(c => c.CompetitionId == competitionId).ToListAsync();

        foreach (var participant in competition.Participants)
        {
            var participantCompetition = participantCompetitions.Find(c => c.ParticipantId == participant.Id);
            participant.TotalScore += participantCompetition.ParticipantScore;
            _context.Entry(participantCompetition).State = EntityState.Modified;
        }
        
        competition.EndDate = DateTime.UtcNow;
        _context.Entry(competition).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        
        return Ok("Competition has been closed and scores have been updated.");
    }
    
    [HttpPost("register-participant-competition/{competitionId}")]
    public async Task<IActionResult> RegisterParticipantCompetition(int competitionId, [FromBody] Participant participant)
    {
        var competition = await _context.Competitions
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == competitionId);
    
        if (competition == null)
            return NotFound("Competition not found");

        if (DateTime.UtcNow > competition.RegistrationDeadline)
            return BadRequest("Registration on this competition has closed");

        var existingParticipant = await _context.Participants
            .FirstOrDefaultAsync(p => p.Id == participant.Id);

        if (existingParticipant == null)
        {
            await _context.Participants.AddAsync(participant);
            await _context.SaveChangesAsync();
            existingParticipant = participant;
        }
        else
        {
            var alreadyRegistered = await _context.ParticipantCompetitions
                .AnyAsync(pc => pc.ParticipantId == existingParticipant.Id && pc.CompetitionId == competitionId);

            if (alreadyRegistered)
                return BadRequest("Participant is already registered for this competition.");
        }

        var participantCompetition = new ParticipantCompetition
        {
            CompetitionId = competitionId,
            ParticipantId = existingParticipant.Id
        };

        await _context.ParticipantCompetitions.AddAsync(participantCompetition);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Participant successfully registered", participantCompetition });
    }

}