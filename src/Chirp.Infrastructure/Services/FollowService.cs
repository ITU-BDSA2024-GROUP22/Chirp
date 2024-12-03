using Chirp.Core;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.DTOs;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Repositories;

public interface IFollowService
{
    Task FollowAuthor(string followerId, string followeeId);
    Task UnfollowAuthor(string followerId, string followeeId);
    Task <bool> IsFollowing(string followerId, string followeeId);
    public List<AuthorDTO> GetFollowing(string user);
}

public class FollowService : IFollowService
{

    private readonly FollowRepository _followRepository;

    public FollowService(FollowRepository followRepository)
    {
        _followRepository = followRepository;
    }

    public async Task FollowAuthor(string followerId, string followeeId)
    {
        // Check if already following to avoid duplication
        if (!await _followRepository.IsFollowing(followerId, followeeId))
        {
            await _followRepository.AddFollow(followerId, followeeId);
        }
    }

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

    public List<AuthorDTO> GetFollowing(string username)
    {
        var authorlist = _followRepository.GetFollowingDTO(username).Result;
        return authorlist;
    }
    public async Task<List<CheepDTO>> GetCheepsFromFollowing(int pageNumber, string username)
    {
        var cheeps = await _followRepository.GetCheepsFromFollowing(pageNumber, username);
        return cheeps;
    }

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
