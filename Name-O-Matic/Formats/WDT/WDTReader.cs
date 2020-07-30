using System.IO;
using System.Runtime.CompilerServices;
using NameOMatic.Constants;
using NameOMatic.Database;
using NameOMatic.Extensions;
using NameOMatic.Helpers.Collections;

namespace NameOMatic.Formats.WDT
{
    internal class WDTReader : IReader<WDTModel>
    {
        public UniqueLookup<string, int> Tokens { get; } = new UniqueLookup<string, int>();

        public bool TryRead(int fileID, out WDTModel model)
        {
            if (!FileContext.Instance.FileExists(fileID))
            {
                model = null;
                return false;
            }

            using var stream = FileContext.Instance.OpenFile(fileID);
            using var reader = new BinaryReader(stream);
            var chunkReader = new IffChunkReader(reader);
            model = new WDTModel(fileID)
            {
                Header = Read<MPHD>(IffToken.MPHD, chunkReader, reader),
                MapIDs = ReadArray<MAID>(IffToken.MAID, chunkReader, reader)
            };

            foreach (var chunk in chunkReader.GetNewTokens(true))
                Tokens.Add(chunk, fileID);

            return true;
        }

        private T Read<T>(IffToken token, IffChunkReader chunkReader, BinaryReader reader) where T : struct
        {
            return chunkReader.GotoChunk(token, out _) ? reader.Read<T>() : default;
        }

        private T[] ReadArray<T>(IffToken token, IffChunkReader chunkReader, BinaryReader reader) where T : struct
        {
            if (chunkReader.GotoChunk(token, out var chunk))
                return reader.ReadArray<T>(chunk.Size / Unsafe.SizeOf<T>());

            return new T[0];
        }
    }
}
