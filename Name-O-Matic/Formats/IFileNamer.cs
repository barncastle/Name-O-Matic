using NameOMatic.Helpers.Collections;

namespace NameOMatic.Formats
{
    interface IFileNamer
    {
        FileNameLookup FileNames { get; }

        FileNameLookup Enumerate();
    }
}
