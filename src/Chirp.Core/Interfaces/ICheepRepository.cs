using Chirp.Core.DTOs;

namespace Chirp.Core.Interfaces;

/// <summary>
/// Interface for the repository that handles the creation, updating, and retrieval of Cheep and Author objects.
/// This interface defines the necessary methods to interact with Cheep-related data.
/// </summary>

public interface ICheepRepository
{
    /// <summary>
    /// Retrieves a list of Cheeps with pagination.
    /// </summary>
    /// <param name="pageNumber">The page number to fetch the corresponding page of Cheeps.</param>
    /// <returns>A list of CheepDTOs for the requested page.</returns>
    public Task<List<CheepDTO>> GetCheeps(int pageNumber);

    /// <summary>
    /// Retrieves a list of Cheeps from a specific author with pagination.
    /// </summary>
    /// <param name="pageNumber">The page number to fetch the corresponding page of Cheeps from the given author.</param>
    /// <param name="username">The username of the author whose Cheeps are to be retrieved.</param>
    /// <returns>A list of CheepDTOs for the requested author and page.</returns>
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string username);

    /// <summary>
    /// Retrieves an Author by their username.
    /// </summary>
    /// <param name="name">The username of the author.</param>
    /// <returns>An AuthorDTO representing the author, or null if no author is found.</returns>
    public Task<AuthorDTO?> GetAuthorByName(string name);

    /// <summary>
    /// Creates a new Author with the specified name and email.
    /// </summary>
    /// <param name="name">The username of the new author.</param>
    /// <param name="email">The email address of the new author.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CreateAuthor(string name, string email);

    /// <summary>
    /// Creates a new Cheep by a specific author with the provided text and timestamp.
    /// </summary>
    /// <param name="authorDTO">The AuthorDTO representing the author.</param>
    /// <param name="text">The text content of the Cheep.</param>
    /// <param name="timeStamp">The timestamp when the Cheep was created.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CreateCheep(AuthorDTO? authorDTO, string text, DateTime timeStamp);

    /// <summary>
    /// Updates the bio of an author.
    /// </summary>
    /// <param name="authorDTO">The AuthorDTO representing the author whose bio will be updated.</param>
    /// <param name="text">The new bio text.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task UpdateBio(AuthorDTO authorDTO, string text);

    /// <summary>
    /// Retrieves the bio of an author by their username.
    /// </summary>
    /// <param name="username">The username of the author whose bio is to be retrieved.</param>
    /// <returns>A Bio object representing the author's bio.</returns>
    public Task<Bio> GetBioFromAuthor(string username);

    /// <summary>
    /// Deletes an author and their associated data (Cheeps, Bio, etc.).
    /// </summary>
    /// <param name="authorDTO">The AuthorDTO representing the author to be deleted.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteAuthor(AuthorDTO authorDTO);

    /// <summary>
    /// Sets the profile picture of an author.
    /// </summary>
    /// <param name="username">The username of the author whose picture will be set.</param>
    /// <param name="picturePath">The file path of the picture to be set.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SetAuthorPictureAsync(string username, string picturePath);
}
