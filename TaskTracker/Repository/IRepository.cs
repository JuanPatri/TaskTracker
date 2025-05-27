namespace Repository;

public interface IRepository <T>
{
    T Add(T entity);

    T? Find(Func<T, bool> predicate);
    
    IList<T> FindAll();
    
    T? Update(T entity);
    
    void Delete(String entity);
}