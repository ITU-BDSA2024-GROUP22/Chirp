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

    /// <summary>
    /// Retrieves a paginated list of Cheeps from the database.
    /// </summary>
    /// <param name="pageNumber"> The page number to retrieve.</param>
    /// <returns> A task representing the asynchronous operation, containing a list of CheepDTO objects.</returns>
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

    /// <summary>
    /// Retrieves a paginated list of Cheeps created by a specific author.
    /// </summary>
    /// <param name="pageNumber"> The page number to retrieve. </param>
    /// <param name="username"> The username of the author whose Cheeps are being retrieved.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of CheepDTO objects
    /// authored by the specified user. </returns>
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

    /// <summary>
    /// Retrieves an author by their username.
    /// </summary>
    /// <param name="name"> The username of the author to retrieve.</param>
    /// <returns> A task representing the asynchronous operation, containing an AuthorDTO object if the author is found,
    /// or null if no author with the specified username exists.</returns>
    public async Task<AuthorDTO?> GetAuthorByName(string name)
    {
        var author = await _dbContext.Authors.SingleOrDefaultAsync(a => a.UserName == name);
        if (author == null)
        {
            return null;
        }
        return AuthorDTO.fromAuthor(author);
    }

    /// <summary>
    /// Creates a new author with the specified username and email, and adds them to the database.
    /// </summary>
    /// <param name="name"> The username of the author to create. </param>
    /// <param name="email"> The email address of the author to create. </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CreateAuthor(string name, string email) //Add id when in use, and returns task instead of void to make the method awaitable
    {
        Author author = new Author (){UserName = name, Email = email, EmailConfirmed = true, FollowingList = new List<Author>() };
        await _dbContext.Authors.AddAsync(author);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a new Cheep with the specified author, text, and timestamp, and adds it to the database.
    /// </summary>
    /// <param name="authorDTO"> The author information, used to retrieve the author from the database.</param>
    /// <param name="text"> The text content of the Cheep.</param>
    /// <param name="timeStamp"> The timestamp of when the Cheep was created.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CreateCheep(AuthorDTO? authorDTO, string text, DateTime timeStamp) //returns task instead of void to make the method awaitable
    {
        var author = _dbContext.Authors.SingleOrDefault(a => a.UserName == authorDTO.UserName);
        if (author != null)
        {
            Cheep cheep = new (){ Author = author, Text = text, TimeStamp = timeStamp};
            await _dbContext.Cheeps.AddAsync(cheep);
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates the bio for a specific author, or creates a new bio if none exists.
    /// </summary>
    /// <param name="authorDTO"> The author whose bio is being updated. </param>
    /// <param name="text"> The new bio text to set for the author. </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateBio(AuthorDTO authorDTO, string text)
    {
        var author = _dbContext.Authors.SingleOrDefault(a => a.UserName == authorDTO.UserName);
        if (author != null)
        {
            var oldBio = _dbContext.Bios.SingleOrDefault(b => b.AuthorId == author.Id);
            if (oldBio != null)
            {
                oldBio.Text = text;
            }
            else
            {
                Bio bio = new() {
                    Author = author,
                    Text = text
                };
                await _dbContext.Bios.AddAsync(bio);
            }
        }
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves the bio of an author by their username.
    /// </summary>
    /// <param name="username"> The username of the author whose bio is being retrieved. </param>
    /// <returns>A task representing the asynchronous operation, containing the author's bio if found,
    /// or null if no bio exists for the specified author.</returns>
    public async Task<Bio> GetBioFromAuthor(string username)
    {
        var bioQuery = _dbContext.Bios
            .Where(bio => bio.Author.UserName == username)
            .OrderByDescending(bio => bio.BioId)
            .Include(bio => bio.Author)
            .Select(bio => new Bio
            {
                Author = bio.Author,
                Text = bio.Text
            });

        return await bioQuery.FirstOrDefaultAsync();
    }
    /// <summary>
    /// Deletes an author, including removing their associated Cheeps, bio, and updating the following lists
    /// of authors who are following them.
    /// </summary>
    /// <param name="authorDTO"> The author to be deleted, identified by their username.</param>
    /// <exception cref="KeyNotFoundException">Thrown if no author with the specified username is found.
    /// </exception>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteAuthor(AuthorDTO authorDTO)
    {
        var authorQuery = _dbContext.Authors
            .Include(a => a.FollowingList)
            .SingleOrDefault(a => a.UserName == authorDTO.UserName);

        if (authorQuery == null)
        {
            throw new KeyNotFoundException($"Author with username '{authorDTO.UserName}' not found.");
        }

        var otherAuthors = await _dbContext.Authors
            .Where(a => a.FollowingList.Contains(authorQuery)) // Find authors following this author
            .ToListAsync();

        foreach (var otherAuthor in otherAuthors)
        {
            otherAuthor.FollowingList.Remove(authorQuery);
        }

        authorQuery.FollowingList.Clear();
        var cheepsQuery = _dbContext.Cheeps
            .Where(cheep => cheep.Author.UserName == authorQuery.UserName);

        if (cheepsQuery.Any())
        {
            Console.WriteLine(cheepsQuery);
            _dbContext.Cheeps.RemoveRange(cheepsQuery);
        }
        var bioQuery = _dbContext.Bios.SingleOrDefault(bio => bio.Author.UserName == authorQuery.UserName);

        if (bioQuery != null)
        {
            _dbContext.Bios.Remove(bioQuery);
        }

        _dbContext.Authors.Remove(authorQuery);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates the profile picture of an author based on their username.
    /// </summary>
    /// <param name="username">The username of the author whose picture is being updated.</param>
    /// <param name="picturePath"> The file path to the new profile picture. </param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if no author with the specified username is found.
    /// </exception>
    ///  <returns> A task representing the asynchronous operation.</returns>
    public async Task SetAuthorPictureAsync(string username, string picturePath)
    {
        var author = await _dbContext.Authors.SingleOrDefaultAsync(a => a.UserName == username);
        if (author == null)
        {
            throw new KeyNotFoundException($"No author with username '{username}' found.");
        }

        author.Picture = picturePath;
        _dbContext.Authors.Update(author);
        await _dbContext.SaveChangesAsync();
    }
}





