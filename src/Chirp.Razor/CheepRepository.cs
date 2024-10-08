using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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

    public async Task<List<CheepDTO>> GetCheeps(int pageNumber)
    {
        var lowerBound = (pageNumber - 1) * pageSize;
        var pageQuery = (from cheep in _dbContext.Cheeps
                orderby cheep.TimeStamp descending
                select cheep)
            .Include(c => c.Author)
            .Skip(lowerBound)
            .Take(pageSize)
            .Select(cheep => new CheepDTO
            {
                Author = cheep.Author.Name,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString(),
            });

        var result = await pageQuery.ToListAsync();

        return result;
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string username)
    {
        var lowerBound = (pageNumber - 1) * pageSize;

        var pageQuery = (from cheep in _dbContext.Cheeps
                where cheep.Author.Name == username // Filter by the author's name
                orderby cheep.TimeStamp descending
                select cheep)
            .Include(c => c.Author)
            .Skip(lowerBound) // Use lowerBound instead of pageNumber * pageSize
            .Take(pageSize)
            .Select(cheep => new CheepDTO
            {
                Author = cheep.Author.Name,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString(),
            });

        var result = await pageQuery.ToListAsync();
        return result;
    }

    public class CheepDTO
    {
        public string Author {get; set;}
        public string Text {get; set;}
        public string TimeStamp {get; set;}
    }

    public class AuthorDTO
    {
        public string Name {get; set;}
        public string Email {get; set;}
    }
}

