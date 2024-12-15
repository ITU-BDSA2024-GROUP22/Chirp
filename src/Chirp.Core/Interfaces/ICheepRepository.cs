using Chirp.Core.DTOs;

namespace Chirp.Core.Interfaces;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheeps(int pageNumber);

    public Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string username);

    public Task<AuthorDTO?> GetAuthorByName(string name);

    public Task CreateAuthor(string name, string email);

    public Task CreateCheep(AuthorDTO authorDTO, string text, DateTime timeStamp);

    public Task UpdateBio(AuthorDTO authorDTO, string text);

    public Task<Bio> GetBioFromAuthor(string username);

    public Task DeleteAuthor(AuthorDTO authorDTO);

    public Task SetAuthorPictureAsync(string username, string picturePath);

    //public Author GetAuthorByEmail(String name);
}
