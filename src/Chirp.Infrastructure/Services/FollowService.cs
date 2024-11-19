using Chirp.Core;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.DTOs;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Repositories;

namespace Chirp.Infrastructure.Services;

public interface IFollowService
{
    Task FollowAuthor(int followerId, int followeeId);
    Task UnfollowAuthor(int followerId, int followeeId);
    Task <bool> IsFollowing(int followerId, int followeeId);
    Task<List<CheepDTO>> GetTimeline(int followerId, int pageNumber);
}

public class FollowService : IFollowService
{

    private readonly CheepRepository _cheepRepository;

    public FollowService(CheepRepository followRepository)
    {
        _cheepRepository = followRepository;
    }

    public async Task FollowAuthor(int followerId, int followeeId)
    {
        // Check if already following to avoid duplication
        if (!await _cheepRepository.IsFollowing(followerId, followeeId))
        {
            await _cheepRepository.AddFollow(followerId, followeeId);
        }
    }

    public async Task UnfollowAuthor(int followerId, int followeeId)
    {
        if (await _cheepRepository.IsFollowing(followerId, followeeId))
        {
            await _cheepRepository.Unfollow(followerId, followeeId);
        }
    }

    public Task<bool> IsFollowing(int followerId, int followeeId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<CheepDTO>> GetTimeline(int followerId, int pageNumber)
    {
        return await _cheepRepository.GetTimeline(followerId, pageNumber);
    }

}
