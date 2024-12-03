
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
        var result = await cheepRepository.GetCheepsFromAuthor(pageNumber, author);

        // Debugging linje til logning
        Console.WriteLine($"Fetched {result.Count} cheeps for {author}");

        return result;
    }

    public async Task<AuthorDTO?> GetAuthorByName(string name)
    {
        var authorDTO = await cheepRepository.GetAuthorByName(name);
        if (authorDTO == null)
        {
            throw new KeyNotFoundException($"No author with name {name} was found.");
        }
        return authorDTO;
    }

    public async Task CreateCheep(AuthorDTO author, string text, DateTime timeStamp)
    {
        await cheepRepository.CreateCheep(author, text, timeStamp);
    }

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

    public async Task SetAuthorPictureAsync(string username, string picturePath)
    {
        await cheepRepository.SetAuthorPictureAsync(username, picturePath);
    }
}


