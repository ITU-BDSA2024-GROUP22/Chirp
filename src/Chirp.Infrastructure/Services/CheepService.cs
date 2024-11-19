
using Chirp.Core;
using Chirp.Core.DTOs;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Repositories;

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int pageNumber);

    public Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int pageNumber);
}

public class CheepService : ICheepService
{
    private readonly CheepRepository cheepRepository;
    // Might need to revisit this implementation of dbContext

    public CheepService(CheepRepository _cheepRepository)
    {
        cheepRepository = _cheepRepository;
    }


    public async Task<List<CheepDTO>> GetCheeps(int pageNumber)
    {
        var results = cheepRepository.GetCheeps(pageNumber);

        return await results;
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author, int pageNumber)
    {
        var results = cheepRepository.GetCheepsFromAuthor(pageNumber, author);
        return await results;
        // filter by the provided author name
        //var cheepSort = results.Where(x => x.Author == author).ToList();
    }

    public async Task<AuthorDTO> GetAuthorByName(string name)
    {
        return await cheepRepository.GetAuthorByName(name);
    }

    public async Task CreateCheep(AuthorDTO author, string text, DateTime timeStamp)
    {
        await cheepRepository.CreateCheep(author, text, timeStamp);
    }

}


