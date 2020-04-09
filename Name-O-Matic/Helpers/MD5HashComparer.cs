using CASCLib;
using System.Collections.Generic;

namespace NameOMatic.Helpers
{
    class CASCLibMD5HashComparer : IEqualityComparer<MD5Hash>
    {
        const uint FnvPrime32 = 16777619;
        const uint FnvOffset32 = 2166136261;

        public unsafe bool Equals(MD5Hash x, MD5Hash y)
        {
            for (int i = 0; i < 16; ++i)
                if (x.Value[i] != y.Value[i])
                    return false;

            return true;
        }

        public unsafe int GetHashCode(MD5Hash obj)
        {
            uint* ptr = (uint*)&obj;

            uint hash = FnvOffset32;
            for (int i = 0; i < 4; i++)
            {
                hash ^= ptr[i];
                hash *= FnvPrime32;
            }
            return unchecked((int)hash);
        }
    }
}
