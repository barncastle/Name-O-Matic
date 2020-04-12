using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using NameOMatic.Constants;
using NameOMatic.Database;
using NameOMatic.Extensions;
using NameOMatic.Helpers;
using NameOMatic.Helpers.Collections;

namespace NameOMatic.Formats.M2
{
    class SkelReader : Singleton<SkelReader>, IReader<SkelModel>
    {
        public UniqueLookup<string, int> Tokens { get; } = new UniqueLookup<string, int>();

        public bool TryRead(int fileID, out SkelModel model)
        {
            if (!FileContext.Instance.FileExists(fileID))
            {
                model = null;
                return false;
            }

            using (var stream = FileContext.Instance.OpenFile(fileID))
            using (var reader = new BinaryReader(stream))
            {
                var chunkReader = new IffChunkReader(reader);

                model = new SkelModel(fileID)
                {
                    AnimFiles = ReadArray<AnimInfo>(IffToken.AFID, chunkReader, reader),
                    BoneFileIds = ReadArray<int>(IffToken.BFID, chunkReader, reader)
                };

                foreach (var chunk in chunkReader.GetNewTokens(true))
                    Tokens.Add(chunk, fileID);

                return true;
            }
        }

        private T[] ReadArray<T>(IffToken token, IffChunkReader chunkReader, BinaryReader reader) where T : struct
        {
            if (chunkReader.GotoChunk(token, out var chunk))
                return reader.ReadArray<T>(chunk.Size / Unsafe.SizeOf<T>());

            return new T[0];
        }
    }
}
