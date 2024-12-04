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

    public async Task<AuthorDTO?> GetAuthorByName(string name)
    {
        var author = await _dbContext.Authors.SingleOrDefaultAsync(a => a.UserName == name);
        if (author == null)
        {
            return null;
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
        Author author = new Author (){UserName = name, Email = email, EmailConfirmed = true, FollowingList = new List<Author>() };
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

    public async Task UpdateBio(AuthorDTO authorDTO, string text)
    {
        var author = _dbContext.Authors.SingleOrDefault(a => a.UserName == authorDTO.UserName);
        if (author != null)
        {
            var oldBio = _dbContext.Bios.SingleOrDefault(b => b.AuthorId == author.Id);
            if (oldBio != null)
            {
                oldBio.Text = text;
            }
            else
            {
                Bio bio = new() {
                    Author = author,
                    Text = text
                };
                await _dbContext.Bios.AddAsync(bio);
            }
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Bio> GetBioFromAuthor(string username)
    {
        var bioQuery = _dbContext.Bios
            .Where(bio => bio.Author.UserName == username)
            .OrderByDescending(bio => bio.BioId)
            .Include(bio => bio.Author)
            .Select(bio => new Bio
            {
                Author = bio.Author,
                Text = bio.Text
            });

        return await bioQuery.FirstOrDefaultAsync();
    }

    public async Task DeleteAuthor(AuthorDTO authorDTO)
    {
        //query getting the author
        var authorQuery = _dbContext.Authors
            .Include(a => a.FollowingList)
            .SingleOrDefault(a => a.UserName == authorDTO.UserName);

        if (authorQuery == null)
        {
            throw new KeyNotFoundException($"Author with username '{authorDTO.UserName}' not found.");
        }

        var otherAuthors = await _dbContext.Authors
            .Where(a => a.FollowingList.Contains(authorQuery)) // Find authors following this author
            .ToListAsync();

        foreach (var otherAuthor in otherAuthors)
        {
            otherAuthor.FollowingList.Remove(authorQuery);
        }

        // Clear the author's FollowingList
        authorQuery.FollowingList.Clear();

        //query getting the author's cheeps
        var cheepsQuery = _dbContext.Cheeps
            .Where(cheep => cheep.Author.UserName == authorQuery.UserName);

        if (cheepsQuery.Any())
        {
            Console.WriteLine(cheepsQuery);
            _dbContext.Cheeps.RemoveRange(cheepsQuery); //removes associated cheeps
        }

        //query getting the author's bio
        var bioQuery = _dbContext.Bios.SingleOrDefault(bio => bio.Author.UserName == authorQuery.UserName);

        if (bioQuery != null)
        {
            _dbContext.Bios.Remove(bioQuery); //removes related bio
        }

        _dbContext.Authors.Remove(authorQuery); //Removes the author

        await _dbContext.SaveChangesAsync(); //saves the changes to the db
    }

    public async Task SetAuthorPictureAsync(string username, string picturePath)
    {
        var author = await _dbContext.Authors.SingleOrDefaultAsync(a => a.UserName == username);
        if (author == null)
        {
            throw new KeyNotFoundException($"No author with username '{username}' found.");
        }

        author.Picture = picturePath;
        _dbContext.Authors.Update(author);
        await _dbContext.SaveChangesAsync();
    }
}





