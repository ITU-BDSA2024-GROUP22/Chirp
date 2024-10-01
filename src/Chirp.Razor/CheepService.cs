using Chirp.Razor;
using DBFacade = Chirp.Razor.DBFacade;

public interface ICheepService
{
    public List<Cheep> GetCheeps(int pageNumber);

    public List<Cheep> GetCheepsFromAuthor(string author, int pageNumber);
}

public class CheepService : ICheepService
{
    private readonly DBFacade facade;

    public CheepService()
    {
        facade = new DBFacade();
        facade.Open();
    }


    public List<Cheep> GetCheeps(int pageNumber)
    {
        var results = facade.GetCheeps(pageNumber);

        return results;
    }

    public List<Cheep> GetCheepsFromAuthor(string author, int pageNumber)
    {
        // filter by the provided author name
        return GetCheeps(pageNumber).Where(x => x.Author == author).ToList();
    }
}