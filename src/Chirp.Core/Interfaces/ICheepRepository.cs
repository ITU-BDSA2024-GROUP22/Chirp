using Chirp.Core.DTOs;

namespace Chirp.Core.Interfaces;

/// <summary>
/// Interface for the repository that handles the creation, updating, and retrieval of Cheep and Author objects.
/// This interface defines the necessary methods to interact with Cheep-related data.
/// </summary>

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheeps(int pageNumber);

    public Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string username);

    public Task<AuthorDTO?> GetAuthorByName(string name);

    public Task CreateAuthor(string name, string email);

    public Task CreateCheep(AuthorDTO? authorDTO, string text, DateTime timeStamp);

    public Task UpdateBio(AuthorDTO authorDTO, string text);

    public Task<Bio> GetBioFromAuthor(string username);

    public Task DeleteAuthor(AuthorDTO authorDTO);

    public Task SetAuthorPictureAsync(string username, string picturePath);
}
