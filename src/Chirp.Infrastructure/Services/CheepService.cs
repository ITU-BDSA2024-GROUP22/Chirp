using Chirp.Core;
using Chirp.Core.DTOs;
using Chirp.Infrastructure.Repositories;

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int pageNumber);

    public Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int pageNumber);

    public Task<AuthorDTO?> GetAuthorByName(string name);

    public Task CreateCheep(AuthorDTO? author, string text, DateTime timeStamp);

    public Task UpdateBio(AuthorDTO authorDTO, string text);

    public Task<Bio> GetBioFromAuthor(string username);

    public Task DeleteAuthor(AuthorDTO authorDTO);

    public Task SetAuthorPictureAsync(string username, string picturePath);
}

public class CheepService : ICheepService
{
    private readonly CheepRepository cheepRepository;

    public CheepService(CheepRepository _cheepRepository)
    {
        cheepRepository = _cheepRepository;
    }


    public async Task<List<CheepDTO>> GetCheeps(int pageNumber)
    {
        var results = cheepRepository.GetCheeps(pageNumber);

        return await results;
    }

    /// <summary>
    /// Retrieves a list of Cheeps for a given author with pagination.
    /// </summary>
    /// <param name="author">The author's username.</param>
    /// <param name="pageNumber">The page number to determine which set of Cheeps to fetch.</param>
    /// <returns>A list of CheepDTO objects for the specified author.</returns>
    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int pageNumber)
    {
        var result = await cheepRepository.GetCheepsFromAuthor(pageNumber, author);

        // Debugging linje til logning
        Console.WriteLine($"Fetched {result.Count} cheeps for {author}");

        return result;
    }

    /// <summary>
    /// Retrieves an author's details by their name.
    /// </summary>
    /// <param name="name">The author's username.</param>
    /// <returns>An AuthorDTO object containing the author's details.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no author with the given name is found.</exception>
    public async Task<AuthorDTO?> GetAuthorByName(string name)
    {
        var authorDTO = await cheepRepository.GetAuthorByName(name);
        if (authorDTO == null)
        {
            throw new KeyNotFoundException($"No author with name {name} was found.");
        }
        return authorDTO;
    }

    /// <summary>
    /// Creates a new Cheep for the specified author with the given text and timestamp.
    /// </summary>
    /// <param name="author">The author creating the Cheep.</param>
    /// <param name="text">The content of the Cheep.</param>
    /// <param name="timeStamp">The timestamp when the Cheep is created.</param>
    /// <remarks>
    /// This method delegates the creation of a Cheep to the repository layer. It ensures that a new Cheep is saved
    /// with the appropriate author and timestamp. The text of the Cheep should be validated elsewhere in the system.
    /// </remarks>
    public async Task CreateCheep(AuthorDTO? author, string text, DateTime timeStamp)
    {
        await cheepRepository.CreateCheep(author, text, timeStamp);
    }

    /// <summary>
    /// Updates the bio for the specified author.
    /// </summary>
    /// <param name="authorDTO">The author whose bio is being updated.</param>
    /// <param name="text">The new bio text.</param>
    /// <remarks>
    /// This method delegates the bio update operation to the repository layer. The bio text should be validated
    /// before calling this method to ensure it meets the application's requirements (e.g., length restrictions).
    /// </remarks>
    public async Task UpdateBio(AuthorDTO authorDTO, string text)
    {
        await cheepRepository.UpdateBio(authorDTO, text);
    }

    public async Task<Bio> GetBioFromAuthor(string username)
    {
        return await cheepRepository.GetBioFromAuthor(username);
    }

    public async Task DeleteAuthor(AuthorDTO authorDTO)
    {
        await cheepRepository.DeleteAuthor(authorDTO);
    }

    /// <summary>
    /// Sets the profile picture for the specified author.
    /// </summary>
    /// <param name="username">The username of the author whose picture is being set.</param>
    /// <param name="picturePath">The file path to the new profile picture.</param>
    /// <remarks>
    /// This method updates the profile picture of the specified author. The picture is stored at the provided
    /// file path. The method assumes that the path is valid and the file exists.
    /// </remarks>
    public async Task SetAuthorPictureAsync(string username, string picturePath)
    {
        await cheepRepository.SetAuthorPictureAsync(username, picturePath);
    }
}


