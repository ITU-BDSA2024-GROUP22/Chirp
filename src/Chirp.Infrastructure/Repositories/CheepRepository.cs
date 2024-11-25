using Chirp.Core;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.DTOs;
using Chirp.Core.Interfaces;

namespace Chirp.Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly int _pageSize = 32;
    private readonly DBContext _dbContext;

    public CheepRepository(DBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CheepDTO>> GetCheeps(int pageNumber)
    {
        var lowerBound = (pageNumber - 1) * _pageSize;
        var pageQuery = (from cheep in _dbContext.Cheeps
                orderby cheep.TimeStamp descending
                select cheep)
            .Include(c => c.Author)
            .Skip(lowerBound)
            .Take(_pageSize)
            .Select(cheep => new CheepDTO
            {
                Author = AuthorDTO.fromAuthor(cheep.Author),
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM/dd/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
            });

        var result = await pageQuery.ToListAsync();

        return result;
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string username)
    {
        var lowerBound = (pageNumber - 1) * _pageSize;

        var pageQuery = (from cheep in _dbContext.Cheeps
                where cheep.Author.UserName == username // Filter by the author's name
                orderby cheep.TimeStamp descending
                select cheep)
            .Include(c => c.Author)
            .Skip(lowerBound) // Use lowerBound instead of pageNumber * pageSize
            .Take(_pageSize)
            .Select(cheep => new CheepDTO
            {
                Author = AuthorDTO.fromAuthor(cheep.Author),
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM/dd/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
            });

        var result = await pageQuery.ToListAsync();
        return result;
    }

    public async Task<AuthorDTO> GetAuthorByName(string name)
    {
        var author =  _dbContext.Authors.SingleOrDefault(a => a.UserName == name);

        if (author == null)
        {
            throw new KeyNotFoundException($"No author with name {name} was found.");
        }

        return AuthorDTO.fromAuthor(author);
    }

    public Author GetAuthorByEmail(string name)
    {
        var author = _dbContext.Authors.SingleOrDefault(a => a.Email == name);

        if (author == null)
        {
            throw new KeyNotFoundException($"No author with name {name} was found.");
        }

        return author;
    }

    public async Task CreateAuthor(string name, string email) //Add id when in use, and returns task instead of void to make the method awaitable
    {
        Author author = new (){UserName = name, Email = email};
        await _dbContext.Authors.AddAsync(author);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateCheep(AuthorDTO authorDTO, string text, DateTime timeStamp) //returns task instead of void to make the method awaitable
    {
        var author = _dbContext.Authors.SingleOrDefault(a => a.UserName == authorDTO.UserName);
        if (author != null)
        {
            Cheep cheep = new (){ Author = author, Text = text, TimeStamp = timeStamp};
            await _dbContext.Cheeps.AddAsync(cheep);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateBio(AuthorDTO authorDTO, string bio)
    {
        var author = _dbContext.Authors.SingleOrDefault(a => a.UserName == authorDTO.UserName);
        if (author == null)
        {
            throw new KeyNotFoundException($"No author with name {authorDTO.DisplayName} was found.");
        }

        author.Bio = bio;
    }

}

