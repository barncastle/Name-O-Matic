using System.Numerics;
using System.Runtime.InteropServices;

namespace NameOMatic.Formats.M2
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct M2Header
    {
        public int Magic;
        public int Version;
        public int LenName;
        public int OfsName;
        public int GlobalFlags;
        public int NGlobalSequences;
        public int OfsGlobalSequences;
        public int NAnimations;
        public int OfsAnimations;
        public int NAnimLookup;
        public int OfsAnimLookup;
        public int NBones;
        public int OfsBones;
        public int NKeyBoneLookup;
        public int OfsKeyBoneLookup;
        public int NVertices;
        public int OfsVertices;
        public int NViews;
        public int NSubmeshAnimations;
        public int OfsSubmeshAnimations;
        public int NTextures;
        public int OfsTextures;
        public int NTransparencies;
        public int OfsTransparencies;
        public int NUvAnimation;
        public int OfsUvAnimation;
        public int NTexReplace;
        public int OfsTexReplace;
        public int NRenderFlags;
        public int OfsRenderFlags;
        public int NBoneLookupTable;
        public int OfsBoneLookupTable;
        public int NTexLookup;
        public int OfsTexLookup;
        public int NTexUnits;
        public int OfsTexUnits;
        public int NTransLookup;
        public int OfsTransLookup;
        public int NUvAnimLookup;
        public int OfsUvAnimLookup;
        public Vector3 VertexBoxMin;
        public Vector3 VertexBoxMax;
        public float VertexRadius;
        public Vector3 BoundingBoxMin;
        public Vector3 BoundingBoxMax;
        public float BoundingRadius;
        public int NBoundingTriangles;
        public int OfsBoundingTriangles;
        public int NBoundingVertices;
        public int OfsBoundingVertices;
        public int NBoundingNormals;
        public int OfsBoundingNormals;
        public int NAttachments;
        public int OfsAttachments;
        public int NAttachLookup;
        public int OfsAttachLookup;
        public int NEvents;
        public int OfsEvents;
        public int NLights;
        public int OfsLights;
        public int NCameras;
        public int OfsCameras;
        public int NCameraLookup;
        public int OfsCameraLookup;
        public int NRibbonEmitters;
        public int OfsRibbonEmitters;
        public int NParticleEmitters;
        public int OfsParticleEmitters;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AnimInfo
    {
        public ushort AnimId;
        public ushort SubAnimId;
        public int AnimFileId;

        public override string ToString() => AnimId.ToString("0000") + "-" + SubAnimId.ToString("00") + ".anim";
    }
}
