using Chirp.Core;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.DTOs;
using Chirp.Core.Interfaces;

namespace Chirp.Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly int _pageSize = 32;
    private readonly DBContext _dbContext;

    public CheepRepository(DBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CheepDTO>> GetCheeps(int pageNumber)
    {
        var lowerBound = (pageNumber - 1) * _pageSize;
        var pageQuery = (from cheep in _dbContext.Cheeps
                orderby cheep.TimeStamp descending
                select cheep)
            .Include(c => c.Author)
            .Skip(lowerBound)
            .Take(_pageSize)
            .Select(cheep => new CheepDTO
            {
                Author = AuthorDTO.fromAuthor(cheep.Author),
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp.ToString("MM/dd/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
            });

        var result = await pageQuery.ToListAsync();

        return result;
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string username)
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
    public async Task<List<AuthorDTO>> GetFollowingDTO(string user)
    {
        // Get the list of authors the user is following
        var followingList = await GetFollowingList(user);

        // Map each Author to an AuthorDTO
        return followingList.Select(author => new AuthorDTO
        {
            UserName = author.UserName,
            DisplayName = string.IsNullOrWhiteSpace(author.DisplayName) ? author.UserName : author.DisplayName,
            FollowingList = new List<Author>() // Optional: Populate this if needed
        }).ToList();
    }


    public async Task<List<Author>> GetFollowingList(string user)
    {
        return await _dbContext.Authors.Where(a => a.UserName == user).Select(a => a.FollowingList)
            .FirstOrDefaultAsync() ?? new List<Author>();
    }


    public async Task<AuthorDTO?> GetAuthorByName(string name)
    {
        var author =  _dbContext.Authors.SingleOrDefault(a => a.UserName == name);

        if (author == null)
        {
            throw new KeyNotFoundException($"No author with name {name} was found. DTO");
        }

        return AuthorDTO.fromAuthor(author);
    }

    public async Task<Author> GetAuthor(string name)
    {
        var author =  _dbContext.Authors.SingleOrDefault(a => a.UserName == name);
        Console.WriteLine("HER ER FEJLEN" + author);

        if (author == null)
        {
            throw new KeyNotFoundException($"No author with name {name} was found.");
        }

        return author;
    }

    public Author GetAuthorByEmail(string name)
    {
        var author = _dbContext.Authors.SingleOrDefault(a => a.Email == name);

        if (author == null)
        {
            throw new KeyNotFoundException($"No author with name {name} was found.");
        }

        return author;
    }

    public async Task CreateAuthor(string name, string email) //Add id when in use, and returns task instead of void to make the method awaitable
    {
        Author author = new Author (){UserName = name, Email = email, EmailConfirmed = true, FollowingList = new List<Author>() };
        await _dbContext.Authors.AddAsync(author);
        Console.WriteLine("YooooooooooooooooooooooooooooO");
        Console.WriteLine($"Author {name} has been created."+ author.DisplayName + author.Email);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateCheep(AuthorDTO authorDTO, string text, DateTime timeStamp) //returns task instead of void to make the method awaitable
    {
        var author = _dbContext.Authors.SingleOrDefault(a => a.UserName == authorDTO.UserName);
        if (author != null)
        {
            Cheep cheep = new (){ Author = author, Text = text, TimeStamp = timeStamp};
            await _dbContext.Cheeps.AddAsync(cheep);
        }

        await _dbContext.SaveChangesAsync();
    }


    public async Task AddFollow(string user, string userFollowed)
    {
        Console.WriteLine("HER ER NR 1" + user);
        Console.WriteLine("HER ER NR 1" + userFollowed);
            var author = await GetAuthor(user);
            var authorFollowed = await GetAuthor(userFollowed);
            if (author.FollowingList.All(a => a.UserName != userFollowed))
            {
                author.FollowingList.Add(authorFollowed);
                await _dbContext.SaveChangesAsync();

                var followedUsers = string.Join(", ", author.FollowingList.Select(a => a.UserName));
                Console.WriteLine($"FollowingList: {followedUsers}");

            }
    }

    public async Task<bool> IsFollowing(string user, string userFollowed)
    {
        var list = await _dbContext.Authors.Where(a => a.UserName == user).Select(a => a.FollowingList)
            .FirstOrDefaultAsync();

        if (list != null && list.Any(a => a.UserName == userFollowed))
        {
            return true;
        }

        return false;
    }

    public async Task Unfollow(string user, string userUnfollowed)
    {
        var authorUnfollowed = await GetAuthor(userUnfollowed);
            _dbContext.Authors.Where(a =>a.UserName == user)
                .Include(a => a.FollowingList).FirstOrDefault()!.FollowingList.Remove(authorUnfollowed);

            await _dbContext.SaveChangesAsync();
        }
    public async Task<List<CheepDTO>> GetCheepsFromFollowing(int pageNumber, string username)
    {
        var lowerBound = (pageNumber - 1) * _pageSize;

        var followingUsernames = (await GetFollowingList(username))
            ?.Select(a => a.UserName)
            .ToList();

        if (followingUsernames == null || !followingUsernames.Any())
        {
            return new List<CheepDTO>();
        }
        var pageQuery = (from cheep in _dbContext.Cheeps
                where followingUsernames.Contains(cheep.Author.UserName)
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

        // Hent og returner resultatet
        var result = await pageQuery.ToListAsync();
        return result;
    }
}





