using Chirp.Core.DTOs;

namespace Chirp.Core.Interfaces;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheeps(int pageNumber);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string username);
    public Task<Author> GetAuthorByName(String name);
    public Author GetAuthorByEmail(String name);
    public Task CreateAuthor(string name, string email);
    public Task CreateCheep(Author author, string text, DateTime timeStamp);
}
