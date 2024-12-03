namespace Chirp.Infrastructure.Repositories;

public class FollowRepository
{
    private readonly DBContext _dbContext;

    public FollowRepository(DBContext dbContext)
    {
        _dbContext = dbContext;
    }


}
