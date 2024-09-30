namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    public List<T> Read(int pageNumber);
    public void Store(T record);
}