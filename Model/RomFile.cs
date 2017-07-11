using System.IO;
using Model.Constants;

namespace Model
{
    public class RomFile
    {
        public CompressionType CompressionType { get; set; }
        public string SourceFile { get; set; }
        public string System { get; set; }
        public string FileName { get; set; }
        public bool IsInCompressedFile { get; set; }
        public string SourceFileName { get; set; }

        public RomFile()
        {
        }
    }
}