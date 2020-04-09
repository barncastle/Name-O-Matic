using System.Collections.Generic;
using System.IO;
using NameOMatic.Constants;

namespace NameOMatic.Formats
{
    class IffChunk
    {
        public readonly IffToken Token;
        public readonly int Size;
        public readonly long Offset;

        public IList<IffChunk> Children { get; set; }

        public IffChunk(BinaryReader reader)
        {
            Token = (IffToken)reader.ReadUInt32();
            Size = reader.ReadInt32();
            Offset = reader.BaseStream.Position;
        }
    }
}
