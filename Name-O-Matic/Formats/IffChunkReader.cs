using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NameOMatic.Constants;
using NameOMatic.Extensions;

namespace NameOMatic.Formats
{
    internal class IffChunkReader : ILookup<IffToken, IffChunk>
    {
        private readonly ILookup<IffToken, IffChunk> _chunks;
        private readonly BinaryReader _reader;

        public IffChunkReader(BinaryReader reader)
        {
            _reader = reader;
            _chunks = IterateChunks(reader).ToLookup(x => x.Token);
            reader.BaseStream.Position = 0;
        }

        #region Interface

        public int Count => _chunks.Count;

        public IEnumerable<IffChunk> this[IffToken key] => _chunks[key];

        public bool Contains(IffToken key) => _chunks.Contains(key);

        public IEnumerator<IGrouping<IffToken, IffChunk>> GetEnumerator() => _chunks.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _chunks.GetEnumerator();

        #endregion Interface

        public bool GotoChunk(IffToken token, out IffChunk chunk)
        {
            chunk = _chunks[token].FirstOrDefault();
            if (chunk == null)
                return false;

            _reader.BaseStream.Position = chunk.Offset;
            return true;
        }

        public IEnumerable<string> GetNewTokens(bool reverse = false)
        {
            return _chunks
                .Select(x => x.Key)
                .Distinct()
                .Where(x => !Enum.IsDefined(typeof(IffToken), x))
                .Select(x => x.ToToken(reverse));
        }

        private IList<IffChunk> IterateChunks(BinaryReader reader, IList<IffChunk> chunks = null, long? length = null)
        {
            chunks = chunks ?? new List<IffChunk>(0x20);
            length = length ?? reader.BaseStream.Length;

            while (length >= 8)
            {
                var chunk = new IffChunk(reader);

                if (chunk.Token > 0 && chunk.Size > 0)
                {
                    chunks.Add(chunk);

                    if (GroupChunks.TryGetValue(chunk.Token, out int headerSize))
                    {
                        chunk.Children = new List<IffChunk>();
                        reader.BaseStream.Position += headerSize;
                        IterateChunks(reader, chunk.Children, chunk.Size - headerSize);
                    }
                    else
                    {
                        reader.BaseStream.Position += chunk.Size;
                    }
                }

                length -= chunk.Size + 8;
            }

            return chunks;
        }

        /// <summary>
        /// Chunks that contain subchunks + their header size
        /// </summary>
        private static readonly Dictionary<IffToken, int> GroupChunks = new Dictionary<IffToken, int>()
        {
            [IffToken.ACNK] = 0x40,
            [IffToken.MCNK] = 0x80,
            [IffToken.MOGP] = 0x40
        };
    }
}
