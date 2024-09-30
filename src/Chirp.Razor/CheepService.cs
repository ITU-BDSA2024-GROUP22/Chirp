using Chirp.CLI;
using Chirp.Razor;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    DBFacade facade;
    public CheepService()
    {
        facade = new DBFacade();
        facade.Open();
    }


    public List<CheepViewModel> GetCheeps(int pageNumber, int pageSize)
    {

        int lowerBound = (pageNumber - 1) * pageSize;

        string query = "SELECT * FROM Cheeps ORDER BY CreatedDate DESC LIMIT {pageSize} OFFSET {lowerBound}";

        return ExecuteQuery(query);
    }


    
    /*
    public List<CheepViewModel> GetCheeps()
    {
        
        return facade.GetCheeps();
    }
    */

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        return GetCheeps().Where(x => x.Author == author).ToList();
    }


}
