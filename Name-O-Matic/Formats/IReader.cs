using NameOMatic.Constants;
using NameOMatic.Helpers.Collections;

namespace NameOMatic.Formats
{
    interface IReader<T> where T : IModel
    {
        UniqueLookup<string, int> Tokens { get; }

        bool TryRead(int fileID, out T model);
    }
}
