using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public interface ICheepRepository;

public class CheepRepository : ICheepRepository
{
    private readonly int pageSize = 32;
    private readonly DBContext _dbContext;

    public CheepRepository(DBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Cheep>> GetCheeps(int pageNumber)
    {
        var lowerBound = (pageNumber - 1) * pageSize;
        var pageQuery = (from cheep in DBContext.messages
                orderby cheep.timeStamp descending
                select cheep)
            .Include(c => c.author)
            .Skip(pageNumber * 32).Take(32);
        var result = await pageQuery.ToListAsync();

        return result;
    }

    public async Task<List<Cheep>> GetCheepsFromAuthor(int pageNumber, string username)
    {
        var lowerBound = (pageNumber - 1) * pageSize;

        var pageQuery = (from cheep in DBContext.messages
                where cheep.author.name == username // Filter by the author's name
                orderby cheep.timeStamp descending
                select cheep)
            .Include(c => c.author)
            .Skip(lowerBound) // Use lowerBound instead of pageNumber * pageSize
            .Take(pageSize);

        var result = await pageQuery.ToListAsync();
        return result;
    }
}

