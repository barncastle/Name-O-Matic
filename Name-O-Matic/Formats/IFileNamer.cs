using NameOMatic.Constants;
using NameOMatic.Helpers.Collections;

namespace NameOMatic.Formats
{
    interface IFileNamer
    {
        string Format { get; }
        bool Enabled { get; }

        FileNameLookup FileNames { get; }
        UniqueLookup<string, int> Tokens { get; }

        FileNameLookup Enumerate();
    }
}
