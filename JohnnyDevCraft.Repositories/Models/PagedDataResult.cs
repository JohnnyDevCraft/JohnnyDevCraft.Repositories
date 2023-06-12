namespace JohnnyDevCraft.Repositories.Models;

public class PagedDataResult<T>
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int Size { get; set; }
    public IEnumerable<T> Data { get; set; }
}