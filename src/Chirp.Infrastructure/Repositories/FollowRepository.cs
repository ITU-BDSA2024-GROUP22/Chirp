using Chirp.Core;
using Chirp.Core.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class FollowRepository
{
    private readonly int _pageSize = 32;
    private readonly DBContext _dbContext;

    public FollowRepository(DBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Author> GetAuthor(string name)
    {
        var author =  _dbContext.Authors.SingleOrDefault(a => a.UserName == name);

        if (author == null)
        {
            throw new KeyNotFoundException($"No author with name {name} was found.");
        }

        return author;
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

    public async Task<List<AuthorDTO>> GetFollowingDTO(string user)
    {
        // Get the list of authors the user is following
        var followingList = await GetFollowingList(user);

        // Map each Author to an AuthorDTO
        return followingList.Select(author => new AuthorDTO
        {
            UserName = author.UserName,
            Email = author.Email,
            FollowingList = new List<Author>() // Optional: Populate this if needed
        }).ToList();
    }

    public async Task<List<Author>> GetFollowingList(string user)
    {
        return await _dbContext.Authors.Where(a => a.UserName == user).Select(a => a.FollowingList)
            .FirstOrDefaultAsync() ?? new List<Author>();
    }

    public async Task AddFollow(string user, string userFollowed)
    {
        var author = await GetAuthor(user);
        var authorFollowed = await GetAuthor(userFollowed);
        if (author.FollowingList.All(a => a.UserName != userFollowed))
        {
            author.FollowingList.Add(authorFollowed);
            await _dbContext.SaveChangesAsync();

            var followedUsers = string.Join(", ", author.FollowingList.Select(a => a.UserName));
        }
    }

    public async Task<bool> IsFollowing(string user, string userFollowed)
    {
        var list = await _dbContext.Authors.Where(a => a.UserName == user).Select(a => a.FollowingList)
            .FirstOrDefaultAsync();

        if (list != null && list.Any(a => a.UserName == userFollowed))
        {
            return true;
        }

        return false;
    }

    public async Task Unfollow(string user, string userUnfollowed)
    {
        var authorUnfollowed = await GetAuthor(userUnfollowed);
        _dbContext.Authors.Where(a =>a.UserName == user)
            .Include(a => a.FollowingList).FirstOrDefault()!.FollowingList.Remove(authorUnfollowed);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<CheepDTO>> GetCheepsFromFollowing(int pageNumber, string username)
    {
        var lowerBound = (pageNumber - 1) * _pageSize;

        var followingUsernames = (await GetFollowingList(username))
            ?.Select(a => a.UserName)
            .ToList();

        var pageQuery = (from cheep in _dbContext.Cheeps
                where followingUsernames.Contains(cheep.Author.UserName) || cheep.Author.UserName == username
                orderby cheep.TimeStamp descending
                select cheep)
            .Include(c => c.Author)
            .Skip(lowerBound)
            .Take(_pageSize)
            .Select(cheep => new CheepDTO
            {
                Author = AuthorDTO.fromAuthor(cheep.Author),
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM/dd/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
            });

        // Hent og returner resultatet
        var result = await pageQuery.ToListAsync();
        return result;
    }

}
