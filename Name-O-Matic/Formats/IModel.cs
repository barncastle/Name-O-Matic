using System.Collections.Generic;

namespace NameOMatic.Formats
{
    interface IModel
    {
        IDictionary<int, string> FileNames { get; }

        void GenerateFileNames();
    }
}
