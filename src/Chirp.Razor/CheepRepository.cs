using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<CheepRepository.CheepDTO>> GetCheeps(int pageNumber);
    public Task<List<CheepRepository.CheepDTO>> GetCheepsFromAuthor(int pageNumber, string username);
    public Author GetAuthorByName(String name);
    public Author GetAuthorByEmail(String name);
    public void CreateAuthor(string name, string email);

}

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
                TimeStamp = cheep.TimeStamp.ToString("MM/dd/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
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
                TimeStamp = cheep.TimeStamp.ToString("MM/dd/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
            });

        var result = await pageQuery.ToListAsync();
        return result;
    }

    public Author GetAuthorByName(string name)
    {
        return _dbContext.Authors.SingleOrDefault(a => a.Name == name);
    }

    public Author GetAuthorByEmail(string name)
    {
        return _dbContext.Authors.SingleOrDefault(a => a.Email == name);
    }

    public async void CreateAuthor(string name, string email) //Add id when in use
    {
        Author author = new (){Name = name, Email = email};
        await _dbContext.Authors.AddAsync(author);
        await _dbContext.SaveChangesAsync();
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

