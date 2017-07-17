using System.IO;
using Model.Constants;

namespace Model
{
    public class RomFile
    {
        public CompressionType CompressionType { get; set; }
        public string SourceFile { get; set; }
        public string SourceFileName { get; set; }
        public string System { get; set; }
        public string FileName { get; set; }
        public string FileNameWiped { get; set; }
        public bool IsInCompressedFile { get; set; }
        public bool Export { get; set; }
        /// <summary>
        /// Number of Parts that wiped out of the name like [!],(b),...
        /// </summary>
        public int WipedPartsCounter { get; set; }
        /// <summary>
        /// Number of found brakets but unknown content like [blubblbublbub] or (fjklsajfalskjflksdf)
        /// </summary>
        public int NotWipedPartsCounter { get; set; }

        public RomFile()
        {
            WipedPartsCounter = 0;
            NotWipedPartsCounter = 0;
            FileNameWiped = string.Empty;
        }
    }
}