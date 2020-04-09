using System.IO;
using NameOMatic.Constants;
using NameOMatic.Database;
using NameOMatic.Extensions;

namespace NameOMatic.Formats.ADTDAT
{
    class ADTDATReader : IReader<ADTDATModel>
    {
        public bool TryRead(int fileID, out ADTDATModel model)
        {
            if (!FileContext.Instance.FileExists(fileID) || ListFile.Instance.ContainsKey(fileID))
            {
                model = null;
                return false;
            }

            using (var stream = FileContext.Instance.OpenFile(fileID))
            using (var reader = new BinaryReader(stream))
            {
                var chunkReader = new IffChunkReader(reader);
                if (!chunkReader.Contains(IffToken.ACNK))
                {
                    model = null;
                    return false;
                }

                model = new ADTDATModel(fileID);

                int i = 0;
                foreach (var chunk in chunkReader[IffToken.ACNK])
                {
                    reader.BaseStream.Position = chunk.Offset;
                    model.Headers[i++] = reader.Read<ACNKHeader>();
                }

                return true;
            }
        }
    }
}
