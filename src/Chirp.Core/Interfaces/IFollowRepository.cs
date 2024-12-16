using Chirp.Core.DTOs;

namespace Chirp.Core.Interfaces;

/// <summary>
/// Interface for the repository that handles follow/unfollow actions and retrieving the following list, cheeps, and author data.
/// This interface defines the necessary methods for managing the relationships between users (authors) and their followers.
/// </summary>
public interface IFollowRepository
{
    /// <summary>
    /// Retrieves an Author by their username.
    /// </summary>
    /// <param name="name">The username of the author.</param>
    /// <returns>An Author object representing the author.</returns>
    public Task<Author> GetAuthor(string name);

    /// <summary>
    /// Retrieves an Author by their username and returns an AuthorDTO.
    /// </summary>
    /// <param name="name">The username of the author.</param>
    /// <returns>An AuthorDTO representing the author, or null if no author is found.</returns>
    public Task<AuthorDTO?> GetAuthorByName(string name);

    /// <summary>
    /// Retrieves a list of Cheeps from a specific author with pagination.
    /// </summary>
    /// <param name="pageNumber">The page number to fetch the corresponding page of Cheeps.</param>
    /// <param name="username">The username of the author whose Cheeps are to be retrieved.</param>
    /// <returns>A list of CheepDTOs for the requested author and page.</returns>
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string username);

    /// <summary>
    /// Retrieves a list of authors that a specific user is following.
    /// </summary>
    /// <param name="user">The username of the user whose following list is to be retrieved.</param>
    /// <returns>A list of AuthorDTOs representing the authors the user is following.</returns>
    public Task<List<AuthorDTO>> GetFollowingDTO(string user);

    /// <summary>
    /// Retrieves a list of authors that a specific user is following.
    /// </summary>
    /// <param name="user">The username of the user whose following list is to be retrieved.</param>
    /// <returns>A list of Author objects representing the authors the user is following.</returns>
    public Task<List<Author>> GetFollowingList(string user);

    /// <summary>
    /// Allows a user to follow another author.
    /// </summary>
    /// <param name="user">The username of the user who will follow another author.</param>
    /// <param name="userFollowed">The username of the author to be followed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task AddFollow(string user, string userFollowed);

    /// <summary>
    /// Checks whether a user is following a specific author.
    /// </summary>
    /// <param name="user">The username of the user.</param>
    /// <param name="userFollowed">The username of the author to check if followed.</param>
    /// <returns>A boolean indicating whether the user is following the author.</returns>
    public Task<bool> IsFollowing(string user, string userFollowed);

    /// <summary>
    /// Allows a user to unfollow another author.
    /// </summary>
    /// <param name="user">The username of the user who will unfollow an author.</param>
    /// <param name="userUnfollowed">The username of the author to be unfollowed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Unfollow(string user, string userUnfollowed);

    /// <summary>
    /// Retrieves a list of Cheeps from authors that a specific user is following.
    /// </summary>
    /// <param name="pageNumber">The page number to fetch the corresponding page of Cheeps from followed authors.</param>
    /// <param name="username">The username of the user whose followed authors' Cheeps are to be retrieved.</param>
    /// <returns>A list of CheepDTOs for the requested authors and page.</returns>
    public Task<List<CheepDTO>> GetCheepsFromFollowing(int pageNumber, string username);
}
