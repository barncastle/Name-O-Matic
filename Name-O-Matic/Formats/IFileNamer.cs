using NameOMatic.Helpers.Collections;

namespace NameOMatic.Formats
{
    internal interface IFileNamer
    {
        string Format { get; }
        bool Enabled { get; }

        FileNameLookup FileNames { get; }
        UniqueLookup<string, int> Tokens { get; }

        FileNameLookup Enumerate();
    }
}
