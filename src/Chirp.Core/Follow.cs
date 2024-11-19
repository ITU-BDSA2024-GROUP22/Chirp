namespace Chirp.Core;

public class Follow
{
    public int Id { get; set; }
    public string FollowerUserId { get; set; }

    public Author Follower { get; set; }
    public string AuthorUserId { get; set; }

    public Author Following { get; set; }

}
