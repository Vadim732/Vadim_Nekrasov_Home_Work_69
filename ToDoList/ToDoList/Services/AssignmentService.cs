using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ToDoList.Models;

namespace ToDoList.Services;

public class AssignmentService
{
    private readonly ToDoListContext _context;
    private readonly IMemoryCache _memoryCache;

    public AssignmentService(ToDoListContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    public async Task Initialize()
    {
        if (!_context.Assignments.Any())
        {
            await _context.Assignments.AddRangeAsync(
                new Assignment
                {
                    Id = 1,
                    Name = "Tea",
                    Description = "Very tasty black tea with bergamot and berries",
                    ArtistName = "Ivan",
                    Priority = 1,
                    Status = 1,
                    DateCreation = DateTime.UtcNow,
                    UserCreatorId = 1
                }
            );
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Assignment>> GetAllAssignmentsCached(string cacheKey)
    {
        if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Assignment> assignments))
        {
            assignments = await _context.Assignments.ToListAsync();
            _memoryCache.Set(cacheKey, assignments, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });
        }
        return assignments;
    }

    public async Task AddAssignment(Assignment assignment)
    {
        _context.Assignments.Add(assignment);
        int n = await _context.SaveChangesAsync();
        if (n > 0)
        {
            _memoryCache.Set(assignment.Id, assignment, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
            });
        }
    }

    public async Task<Assignment> GetAssignment(int id)
    {
        if (!_memoryCache.TryGetValue(id, out Assignment assignment))
        {
            assignment = await _context.Assignments.FindAsync(id);
            if (assignment != null)
            {
                _memoryCache.Set(assignment.Id, assignment, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
                });
            }
        }
        return assignment;
    }
}
