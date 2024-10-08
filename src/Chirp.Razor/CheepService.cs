/*
using Chirp.Razor;
using DBFacade = Chirp.Razor.DBFacade;

public interface ICheepService
{
    public Task<List<Cheep>> GetCheeps(int pageNumber);

    public Task<List<Cheep>> GetCheepsFromAuthor(string author, int pageNumber);
}

public class CheepService : ICheepService
{
    private readonly DBFacade facade;


    public CheepService()
    {
        facade = new DBFacade();
        //facade.Open();
    }


    public Task<List<Cheep>> GetCheeps(int pageNumber)
    {
        var results = facade.GetCheeps(pageNumber);

        return results;
    }

    public Task<List<Cheep>> GetCheepsFromAuthor(string author, int pageNumber)
    {
        var results = facade.GetCheepsFromAuthor(pageNumber, author);
        return results;
        // filter by the provided author name
        //var cheepSort = results.Where(x => x.Author == author).ToList();
    }
}
*/
