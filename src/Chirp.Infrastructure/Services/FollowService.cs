using Chirp.Core;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.DTOs;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Repositories;

namespace Chirp.Infrastructure.Services;

public interface IFollowService
{
    Task FollowAuthor(string followerId, string followeeId);
    Task UnfollowAuthor(string followerId, string followeeId);
    Task <bool> IsFollowing(string followerId, string followeeId);
    public List<AuthorDTO> GetFollowing(string user);
}

public class FollowService : IFollowService
{

    private readonly CheepRepository _cheepRepository;

    public FollowService(CheepRepository followRepository)
    {
        _cheepRepository = followRepository;
    }

    public async Task FollowAuthor(string followerId, string followeeId)
    {
        // Check if already following to avoid duplication
        if (!await _cheepRepository.IsFollowing(followerId, followeeId))
        {
            await _cheepRepository.AddFollow(followerId, followeeId);
        }
    }

    public async Task UnfollowAuthor(string followerId, string followeeId)
    {
        if (await _cheepRepository.IsFollowing(followerId, followeeId))
        {
            await _cheepRepository.Unfollow(followerId, followeeId);
        }
    }

    public Task<bool> IsFollowing(string followerId, string followeeId)
    {
        throw new NotImplementedException();
    }

    public List<AuthorDTO> GetFollowing(string username)
    {
        var authorlist = _cheepRepository.GetFollowings(username);
        return authorlist;
    }

}
