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
        Author author = new Author (){UserName = name, Email = email, EmailConfirmed = true, FollowingList = new List<Follow>(), FollowersList = new List<Follow>() };
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

    public async Task<List<CheepDTO>>GetCheepsFromFollowedUsers(Author author, int pageNumber)
    {
        var ifollow = await _dbContext.Follows
            .Where(f => f.FollowerUserId == author.Id)
            .Select(f => f.AuthorUserId)
            .ToListAsync();

        return await _dbContext.Cheeps
            .Where(c => c.AuthorId != null && ifollow.Contains(c.AuthorId))
            .OrderByDescending(c => c.TimeStamp)
            .Skip((pageNumber - 1) * _pageSize)
            .Take(_pageSize)
            .Select(cheep => new CheepDTO
            {
                Author = AuthorDTO.fromAuthor(cheep.Author),
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM/dd/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
            })
            .ToListAsync();
    }

    public async Task AddFollow(string followerId, string followeeId)
    {
        if (!await IsFollowing(followerId, followeeId))
        {
            _dbContext.Follows.Add(new Follow
            {
                FollowerUserId = followerId,
                AuthorUserId = followeeId,
            });
            await _dbContext.SaveChangesAsync();
        }

    }

    public async Task<bool> IsFollowing(string followerId, string followeeId)
    {
        return await _dbContext.Follows
            .AnyAsync(f => f.FollowerUserId == followerId && f.AuthorUserId == followeeId);
    }

    public async Task Unfollow(string followerId, string followeeId)
    {
        var follow = await _dbContext.Follows
            .FirstOrDefaultAsync(f => f.FollowerUserId== followerId && f.AuthorUserId == followeeId);

        if (follow != null)
        {
            _dbContext.Follows.Remove(follow);
            await _dbContext.SaveChangesAsync();
        }
    }

    }


