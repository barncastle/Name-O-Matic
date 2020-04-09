namespace NameOMatic.Formats
{
    interface IReader<T> where T : IModel
    {
        bool TryRead(int fileID, out T model);
    }
}
