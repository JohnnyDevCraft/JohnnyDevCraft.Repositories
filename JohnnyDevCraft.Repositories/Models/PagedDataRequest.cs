namespace JohnnyDevCraft.Repositories.Models;

public class PagedDataRequest<T>
{
    public int Page { get; set; }
    public int Size { get; set; }
    public Func<IQueryable<T>, IQueryable<T>> OrderBy { get; set; }
}