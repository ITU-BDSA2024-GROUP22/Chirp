using Chirp.Core.DTOs;

namespace Chirp.Core.Interfaces;

/// <summary>
/// Interface for the repository that handles follow/unfollow actions and retrieving the following list, cheeps, and author data.
/// This interface defines the necessary methods for managing the relationships between users (authors) and their followers.
/// </summary>
public interface IFollowRepository
{

    public Task<Author> GetAuthor(string name);

    public Task<AuthorDTO?> GetAuthorByName(string name);

    public Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string username);

    public Task<List<AuthorDTO>> GetFollowingDTO(string user);

    public Task<List<Author>> GetFollowingList(string user);

    public Task AddFollow(string user, string userFollowed);

    public Task<bool> IsFollowing(string user, string userFollowed);

    public Task Unfollow(string user, string userUnfollowed);

    public Task<List<CheepDTO>> GetCheepsFromFollowing(int pageNumber, string username);
}
