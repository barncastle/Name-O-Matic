using System.Numerics;
using System.Runtime.InteropServices;

namespace NameOMatic.Formats.WMO
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WMOHeader
    {
        public int nTextures;
        public int nGroups;
        public int nPortals;
        public int nLights;
        public int nDoodadNames;
        public int nDoodadDefs;
        public int nDoodadSets;
        public uint ambColor;
        public int wmoID;
        public Vector3 bboxMin;
        public Vector3 bboxMax;
        public ushort flags;
        public ushort NLod;

        public int LodCount => (flags & 16) == 0 ? 1 : (NLod > 0 ? NLod : 3);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MOMT
    {
        public readonly uint flags;
        public readonly int shader;
        public readonly int blendMode;
        public readonly int texture1;
        public readonly uint sidnColor;
        public readonly uint frameSidnColor;
        public readonly int texture2;
        public readonly uint diffColor;
        public readonly uint groundType;
        public readonly int texture3;
        public readonly uint unkColor;
        public readonly uint unkFlags;
        private readonly int padding1, padding2, padding3, padding4;
    }
}
