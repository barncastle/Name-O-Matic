using System.Runtime.InteropServices;

namespace NameOMatic.Formats.WDT
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MPHD
    {
        public int Flags;
        public int LgtFile;
        public int OccFile;
        public int FogsFile;
        public int MpvFile;
        public int TexFile;
        public int WdlFile;
        public int PD4File;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MAID
    {
        public int RootADT;
        public int Obj0ADT;
        public int Obj1ADT;
        public int Tex0ADT;
        public int LodADT;
        public int MapTexture;
        public int MapTextureN;
        public int MinimapTexture;
    }
}
