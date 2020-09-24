using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NameOMatic.Database;
using static TagLib.File;

namespace NameOMatic.Formats.MP3
{
    internal class MP3FileAbstraction : IFileAbstraction
    {
        public string Name { get; }
        public Stream ReadStream { get; }
        public Stream WriteStream => throw new NotImplementedException();

        public MP3FileAbstraction(int fileID)
        {
            Name = $"{fileID}.mp3";
            ReadStream = FileContext.Instance.OpenFile(fileID);
        }

        public void CloseStream(Stream stream) => ReadStream.Close();
    }
}
