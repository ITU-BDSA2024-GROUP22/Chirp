using Chirp.Core;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.DTOs;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Repositories;

public interface IFollowService
{
    public Task FollowAuthor(string followerId, string followeeId);
    public Task UnfollowAuthor(string followerId, string followeeId);
    public Task <bool> IsFollowing(string followerId, string followeeId);
    public List<AuthorDTO> GetFollowing(string user);
    public Task<List<CheepDTO>> GetCheepsFromFollowing(int pageNumber, string username);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int pageNumber);
    public Task<AuthorDTO> GetAuthorByName(string name);
}

/// <summary>
/// Service that relays information regarding Authors and their followers to 'FollowRepository',
/// such that the repository can perform these operations, and it ensures data integrity and validation at each step.
/// Implements the IFollowService interface
/// </summary>
public class FollowService : IFollowService
{

    private readonly FollowRepository _followRepository;

    public FollowService(FollowRepository followRepository)
    {
        _followRepository = followRepository;
    }

    /// <summary>
    /// Allows a user to follow another author, preventing duplicate follow entries
    /// </summary>
    /// <param name="followerId">The ID of the user who wants to follow</param>
    /// <param name="followeeId">The ID of the author being followed</param>
    /// <remarks>
    /// This method checks if the user is already following the specified author and only adds the follow
    /// relationship if it doesn't already exist. This prevents duplicate entries in the database
    /// </remarks>
    public async Task FollowAuthor(string followerId, string followeeId)
    {
        if (!await _followRepository.IsFollowing(followerId, followeeId))
        {
            await _followRepository.AddFollow(followerId, followeeId);
        }
    }

    /// <summary>
    /// Allows a user to unfollow another author, removing the follow relationship if it exists
    /// </summary>
    /// <param name="followerId">The ID of the user who wants to unfollow</param>
    /// <param name="followeeId">The ID of the author to be unfollowed</param>
    /// <remarks>
    /// This method checks if the user is currently following the author before removing the relationship.
    /// It prevents errors by ensuring that no unfollow action is attempted unless the user is following the author
    /// </remarks>
    public async Task UnfollowAuthor(string followerId, string followeeId)
    {
        if (await _followRepository.IsFollowing(followerId, followeeId))
        {
            await _followRepository.Unfollow(followerId, followeeId);
        }
    }

    public async Task<bool> IsFollowing(string followerId, string followeeId)
    {
        return await _followRepository.IsFollowing(followerId, followeeId);
    }

    /// <summary>
    /// Retrieves a list of authors that the specified user is following
    /// </summary>
    /// <param name="username">The username of the user whose following list is being retrieved</param>
    /// <returns>A list of AuthorDTO objects representing the authors the user is following</returns>
    /// <remarks>
    /// This method synchronously retrieves the following list for a given user. Consider using async/await
    /// instead of .Result to avoid blocking the thread during the database query.
    /// </remarks>
    public List<AuthorDTO> GetFollowing(string username)
    {
        var authorlist = _followRepository.GetFollowingDTO(username).Result;
        return authorlist;
    }

    /// <summary>
    /// Retrieves a list of Cheeps from authors that the specified user is following, with pagination
    /// </summary>
    /// <param name="pageNumber">The page number used for pagination to fetch a specific set of Cheeps</param>
    /// <param name="username">The username of the user whose following Cheeps are being retrieved</param>
    /// <returns>A list of CheepDTO objects from the authors the user is following</returns>
    /// <remarks>
    /// This method fetches Cheeps from authors the user is following. Pagination ensures that only a subset
    /// of Cheeps is returned per request, improving performance when fetching large amounts of Cheeps.
    /// </remarks>
    public async Task<List<CheepDTO>> GetCheepsFromFollowing(int pageNumber, string username)
    {
        var cheeps = await _followRepository.GetCheepsFromFollowing(pageNumber, username);
        return cheeps;
    }

    /// <summary>
    /// Retrieves a list of Cheeps from a specific author, with pagination support
    /// </summary>
    /// <param name="author">The username of the author whose Cheeps are being retrieved</param>
    /// <param name="pageNumber">The page number used for pagination to fetch a specific set of Cheeps</param>
    /// <returns>A list of CheepDTO objects from the specified author</returns>
    /// <remarks>
    /// This method retrieves Cheeps from a specific author, supporting pagination. A debug line is included
    /// to log the number of Cheeps retrieved, which can be helpful for debugging
    /// </remarks>
    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int pageNumber)
    {
        var result = await _followRepository.GetCheepsFromAuthor(pageNumber, author);

        // Debugging linje til logning
        Console.WriteLine($"Fetched {result.Count} cheeps for {author}");

        return result;
    }

    public async Task<AuthorDTO> GetAuthorByName(string name)
    {
        return await _followRepository.GetAuthorByName(name);
    }
}
