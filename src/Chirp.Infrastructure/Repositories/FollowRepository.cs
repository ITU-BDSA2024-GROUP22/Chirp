using Chirp.Core;
using Chirp.Core.DTOs;
using Chirp.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly int _pageSize = 32;
    private readonly DBContext _dbContext;

    public FollowRepository(DBContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves an author by their username.
    /// </summary>
    /// <param name="name"> The username of the author to retrieve. </param>
    /// <returns>A task representing the asynchronous operation, containing the author if found.</returns>
    /// <exception cref="KeyNotFoundException"> Thrown if no author with the specified username is found. </exception>
    public override async Task<Author> GetAuthor(string name)
    {
        var author = _dbContext.Authors.SingleOrDefault(a => a.UserName == name);
        if (author == null)
        {
            throw new KeyNotFoundException($"No author with name {name} was found.");
        }

        return author;
    }

    /// <summary>
    /// Retrieves an author by their username and returns an AuthorDTO representation.
    /// </summary>
    /// <param name="name"> The username of the author to retrieve. </param>
    /// <returns> A task representing the asynchronous operation, containing an AuthorDTO if the author is found,
    /// or null if no author with the specified username exists. </returns>
    public override async Task<AuthorDTO?> GetAuthorByName(string name)
    {
        var author = await _dbContext.Authors.SingleOrDefaultAsync(a => a.UserName == name);
        if (author == null)
        {
            return null;
        }
        return AuthorDTO.fromAuthor(author);
    }


    /// <summary>
    /// Retrieves a paginated list of Cheeps from a specific author based on their username.
    /// </summary>
    /// <param name="pageNumber"> The page number to retrieve. </param>
    /// <param name="username"> The username of the author whose Cheeps are being retrieved.</param>
    /// <returns> A task representing the asynchronous operation, containing a list of CheepDTO objects
    /// created by the specified user.</returns>
    public override async Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string username)
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

    /// <summary>
    /// Retrieves a list of AuthorDTO objects representing the authors that a specific user is following.
    /// </summary>
    /// <param name="user"> The username of the user whose following list is being retrieved.</param>
    /// <returns> A task representing the asynchronous operation, containing a list of AuthorDTO objects
    /// representing the authors the user is following.</returns>
    public override async Task<List<AuthorDTO>> GetFollowingDTO(string user)
    {
        var followingList = await GetFollowingList(user);
        return followingList.Select(author => new AuthorDTO
        {
            UserName = author.UserName,
            Email = author.Email,
            FollowingList = new List<Author>()
        }).ToList();
    }

    /// <summary>
    /// Retrieves the list of authors that a specific user is following.
    /// </summary>
    /// <param name="user"> The username of the user whose following list is being retrieved.</param>
    /// <returns> A task representing the asynchronous operation, containing a list of authors that the user is following.
    /// If the user is not following anyone, an empty list is returned. </returns>
    public override async Task<List<Author>> GetFollowingList(string user)
    {
        return await _dbContext.Authors.Where(a => a.UserName == user).Select(a => a.FollowingList)
            .FirstOrDefaultAsync() ?? new List<Author>();
    }


    /// <summary>
    /// Adds a user to another user's following list if they are not already following them.
    /// </summary>
    /// <param name="user"> The username of the user who wants to follow someone </param>
    /// <param name="userFollowed"> The username of the user to be followed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task AddFollow(string user, string userFollowed)
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

    /// <summary>
    /// Checks if a user is following another user.
    /// </summary>
    /// <param name="user"> The username of the user who may be following someone. </param>
    /// <param name="userFollowed"> The username of the user being checked for in the following list.</param>
    /// <returns> A task representing the asynchronous operation, returning true if the user is following the specified user,
    /// otherwise false.</returns>
    public override async Task<bool> IsFollowing(string user, string userFollowed)
    {
        var list = await _dbContext.Authors.Where(a => a.UserName == user).Select(a => a.FollowingList)
            .FirstOrDefaultAsync();

        if (list != null && list.Any(a => a.UserName == userFollowed))
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Removes a user from another user's following list, effectively unfollowing them.
    /// </summary>
    /// <param name="user"> The username of the user who is unfollowing someone. </param>
    /// <param name="userUnfollowed"> The username of the user to be unfollowed. </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task Unfollow(string user, string userUnfollowed)
    {
        var authorUnfollowed = await GetAuthor(userUnfollowed);
        _dbContext.Authors.Where(a =>a.UserName == user)
            .Include(a => a.FollowingList).FirstOrDefault()!.FollowingList.Remove(authorUnfollowed);

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves a paginated list of Cheeps from the users that a specific user is following including their own Cheeps
    /// </summary>
    /// <param name="pageNumber"> The page number to retrieve. </param>
    /// <param name="username"> The username of the user whose followed users' Cheeps are being retrieved.</param>
    /// <returns> A task representing the asynchronous operation, containing a list of CheepDTO objects from the followed users including its own Cheeps.
    /// </returns>
    public override async Task<List<CheepDTO>> GetCheepsFromFollowing(int pageNumber, string username)
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

        var result = await pageQuery.ToListAsync();
        return result;
    }

}
