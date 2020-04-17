using System;
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
    class M2Reader : Singleton<M2Reader>, IReader<M2Model>
    {
        public M2Guesstimator DirectoryAnalyser { get; } = new M2Guesstimator();
        public UniqueLookup<string, int> Tokens { get; } = new UniqueLookup<string, int>();

        public bool TryRead(int fileID, out M2Model model)
        {
            if (!FileContext.Instance.FileExists(fileID))
            {
                model = null;
                return false;
            }

            using var stream = FileContext.Instance.OpenFile(fileID);
            using var reader = new BinaryReader(stream);
            var chunkReader = new IffChunkReader(reader);
            if (!chunkReader.GotoChunk(IffToken.MD21, out _))
            {
                model = null;
                return false;
            }

            model = ParseMD20(reader, fileID);
            if (chunkReader.GotoChunk(IffToken.LDV1, out _))
                ReadLDV1(reader, model);
            if (chunkReader.GotoChunk(IffToken.SFID, out var sfidChunk))
                ReadSFID(reader, sfidChunk, model);

            model.TextureFileIds = ReadArray<int>(IffToken.TXID, chunkReader, reader);
            model.AnimFiles = ReadArray<AnimInfo>(IffToken.AFID, chunkReader, reader);
            model.BoneFileIds = ReadArray<int>(IffToken.BFID, chunkReader, reader);
            model.PhysFileId = Read<int>(IffToken.PFID, chunkReader, reader);
            model.SkelFileId = Read<int>(IffToken.SKID, chunkReader, reader);

            if (!ListFile.Instance.ContainsKey(fileID))
                DirectoryAnalyser.Analyse(model);

            foreach (var chunk in chunkReader.GetNewTokens(true))
                Tokens.Add(chunk, fileID);

            return true;
        }


        private M2Model ParseMD20(BinaryReader reader, int fileId)
        {
            var header = reader.Read<M2Header>();
            reader.BaseStream.Position = header.OfsName + 8;
            string internalName = reader.ReadString(header.LenName - 1);

            return new M2Model(fileId, internalName)
            {
                NViews = header.NViews
            };
        }

        private void ReadLDV1(BinaryReader reader, M2Model model)
        {
            reader.BaseStream.Position += 2;
            model.NLod = reader.ReadUInt16() - 1;
        }

        private void ReadSFID(BinaryReader reader, IffChunk chunk, M2Model model)
        {
            // NOTE: M2s don't always agree to the NViews/NLod logic need to validate the chunk size
            int count = chunk.Size / 4;
            int views = Math.Min(model.NViews, count);
            model.SkinFileIds = reader.ReadArray<int>(views);
            model.LodSkinFileIds = reader.ReadArray<int>(Math.Min(model.NLod, count - views));
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
