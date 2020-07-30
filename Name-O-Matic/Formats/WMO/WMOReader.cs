using System.IO;
using System.Runtime.CompilerServices;
using NameOMatic.Constants;
using NameOMatic.Database;
using NameOMatic.Extensions;
using NameOMatic.Helpers.Collections;

namespace NameOMatic.Formats.WMO
{
    internal class WMOReader : IReader<WMOModel>
    {
        public UniqueLookup<string, int> Tokens { get; } = new UniqueLookup<string, int>();

        public bool TryRead(int fileID, out WMOModel model)
        {
            if (!FileContext.Instance.FileExists(fileID))
            {
                model = null;
                return false;
            }

            using var stream = FileContext.Instance.OpenFile(fileID);
            using var reader = new BinaryReader(stream);
            var chunkReader = new IffChunkReader(reader);
            if (!chunkReader.GotoChunk(IffToken.MOHD, out _))
            {
                model = null;
                return false;
            }

            model = ParseMOHD(reader, fileID);
            model.GroupFileDataId = ReadArray<int>(IffToken.GFID, chunkReader, reader);
            model.Materials = ReadArray<MOMT>(IffToken.MOMT, chunkReader, reader);

            foreach (var chunk in chunkReader.GetNewTokens())
                Tokens.Add(chunk, fileID);

            return true;
        }

        private WMOModel ParseMOHD(BinaryReader reader, int fileId)
        {
            var header = reader.Read<WMOHeader>();
            return new WMOModel(fileId, header.wmoID)
            {
                NGroups = header.nGroups,
                NLod = header.LodCount
            };
        }

        private T[] ReadArray<T>(IffToken token, IffChunkReader chunkReader, BinaryReader reader) where T : struct
        {
            if (chunkReader.GotoChunk(token, out var chunk))
                return reader.ReadArray<T>(chunk.Size / Unsafe.SizeOf<T>());

            return new T[0];
        }
    }
}
