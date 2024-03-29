﻿using System.Collections.Generic;

namespace NameOMatic.Formats.DB2
{
    internal interface IDB2
    {
        IDictionary<int, string> FileNames { get; }

        bool IsValid { get; }

        void Enumerate();
    }
}
