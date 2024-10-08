using System.Globalization;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace Chirp.Razor;

public class DBFacade
{
    // var sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";

    private readonly SqliteConnection connection;
    private readonly int pageSize = 32;
    private readonly DBContext dbContext;

    public DBFacade(DBContext dbContext)
    {
    }

    public async Task<List<Cheep>> GetCheeps(int pageNumber)
    {
            var lowerBound = (pageNumber - 1) * pageSize;
            var pageQuery = (from cheep in DBContext.messages
                        orderby cheep.timeStamp descending
                        select cheep)
                    .Include(c => c.author)
                    .Skip(pageNumber * 32).Take(32);
                var result = await pageQuery.ToListAsync();

                return result;
    }

    public async Task<List<Cheep>> GetCheepsFromAuthor(int pageNumber, string username)
    {
        var lowerBound = (pageNumber - 1) * pageSize;

        var pageQuery = (from cheep in DBContext.messages
                where cheep.author.name == username // Filter by the author's name
                orderby cheep.timeStamp descending
                select cheep)
            .Include(c => c.author)
            .Skip(lowerBound) // Use lowerBound instead of pageNumber * pageSize
            .Take(pageSize);

        var result = await pageQuery.ToListAsync();
        return result;
        }
}


