using Chirp.Core.DTOs;

namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheeps(int pageNumber);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNumber, string username);
    public Author GetAuthorByName(String name);
    public Author GetAuthorByEmail(String name);
    public void CreateAuthor(string name, string email);
    public void CreateCheep(Author author, string text, DateTime timeStamp);
}
