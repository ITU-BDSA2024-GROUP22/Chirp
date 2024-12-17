using Chirp.Core.DTOs;

namespace Chirp.Core.Interfaces;

/// <summary>
/// Interface for the repository that handles follow/unfollow actions and retrieving the following list, cheeps, and author data.
/// This interface defines the necessary methods for managing the relationships between users (authors) and their followers.
/// </summary>
public abstract class IFollowRepository
{

    public abstract Task<Author> GetAuthor(string name);

    public abstract Task<AuthorDTO?> GetAuthorByName(string name);

    public abstract Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string username);

    public abstract Task<List<AuthorDTO>> GetFollowingDTO(string user);

    public abstract Task<List<Author>> GetFollowingList(string user);

    public abstract Task AddFollow(string user, string userFollowed);

    public abstract Task<bool> IsFollowing(string user, string userFollowed);

    public abstract Task Unfollow(string user, string userUnfollowed);

    public abstract Task<List<CheepDTO>> GetCheepsFromFollowing(int pageNumber, string username);
}
