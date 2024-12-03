using Chirp.Core.DTOs;

namespace Chirp.Core.Interfaces;

public interface IFollowRepository
{
    public Task FollowAuthor(string followerId, string followeeId);
    public Task UnfollowAuthor(string followerId, string followeeId);
    public Task <bool> IsFollowing(string followerId, string followeeId);
    public List<AuthorDTO> GetFollowing(string user);
}
