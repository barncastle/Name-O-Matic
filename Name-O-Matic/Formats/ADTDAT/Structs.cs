using System.Runtime.InteropServices;

namespace NameOMatic.Formats.ADTDAT
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ACNKHeader
    {
        public int IndexX;
        public int IndexY;
        public uint Flags;
        public int AreaId;
        public short HolesLowRes;
        public fixed uint PredTexture[4];
        public uint NoEffectDoodad;
        public fixed byte Unknown1[6];
        public ushort UnknownButUsed;
        public fixed byte HolesHighRes[8];
        public fixed byte Unknown2[6];
    }
}
