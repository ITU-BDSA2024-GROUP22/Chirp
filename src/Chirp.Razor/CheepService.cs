using Chirp.Razor;

public interface ICheepService
{
    public Task<List<CheepRepository.CheepDTO>> GetCheeps(int pageNumber);

    public Task<List<CheepRepository.CheepDTO>> GetCheepsFromAuthor(string author, int pageNumber);
}

public class CheepService : ICheepService
{
    private readonly CheepRepository cheepRepository;

    private readonly DBContext dbContext;
    // Might need to revisit this implementation of dbContext

    public CheepService()
    {
        cheepRepository = new CheepRepository(dbContext);
    }


    public async Task<List<CheepRepository.CheepDTO>> GetCheeps(int pageNumber)
    {
        var results = cheepRepository.GetCheeps(pageNumber);

        return await results;
    }

    public async Task<List<CheepRepository.CheepDTO>> GetCheepsFromAuthor(string author, int pageNumber)
    {
        var results = cheepRepository.GetCheepsFromAuthor(pageNumber, author);
        return await results;
        // filter by the provided author name
        //var cheepSort = results.Where(x => x.Author == author).ToList();
    }
}
