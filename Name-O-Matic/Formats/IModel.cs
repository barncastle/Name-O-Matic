using System.Collections.Generic;

namespace NameOMatic.Formats
{
    internal interface IModel
    {
        IDictionary<int, string> FileNames { get; }

        void GenerateFileNames();
    }
}
