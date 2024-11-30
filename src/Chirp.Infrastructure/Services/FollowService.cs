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

    public async Task<bool> IsFollowing(string followerId, string followeeId)
    {
        return await _cheepRepository.IsFollowing(followerId, followeeId);
    }

    public List<AuthorDTO> GetFollowing(string username)
    {
        var authorlist = _cheepRepository.GetFollowingDTO(username).Result;
        return authorlist;
    }
    public async Task<List<CheepDTO>> GetCheepsFromFollowing(int pageNumber, string username)
    {
        var cheeps = await _cheepRepository.GetCheepsFromFollowing(pageNumber, username);
        return cheeps;
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int pageNumber)
    {
        var result = await _cheepRepository.GetCheepsFromAuthor(pageNumber, author);

        // Debugging linje til logning
        Console.WriteLine($"Fetched {result.Count} cheeps for {author}");

        return result;
    }

    public async Task<AuthorDTO> GetAuthorByName(string name)
    {
        return await _cheepRepository.GetAuthorByName(name);
    }




}
