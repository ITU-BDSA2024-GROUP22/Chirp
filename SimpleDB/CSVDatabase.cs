using System.Collections.Generic;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    
    public IEnumerable<T> Read(int? limit = null)
    {
        
    }

    public void Store(T record)
    {
       
    }
    
}