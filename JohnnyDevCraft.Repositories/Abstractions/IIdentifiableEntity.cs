namespace JohnnyDevCraft.Repositories.Abstractions;

public interface IIdentifiableEntity<TKey>
{
    TKey Id { get; set; }
}