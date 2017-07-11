using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Constants;
using SharpCompress.Archives;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;
using SharpCompress.Readers;

namespace RomSetManager
{
    public class RomFileReader
    {
        public List<RomFile> Read(string sourceFile)
        {
            var directory = Path.GetDirectoryName(sourceFile);
            
            var romFile = new RomFile
            {
                CompressionType = GetCompressionType(sourceFile), 
                SourceFile = sourceFile,
                SourceFileName = Path.GetFileName(sourceFile),
                System = Path.GetFileName(directory)
            };

            if (romFile.CompressionType == CompressionType.None)
            {
                romFile.IsInCompressedFile = false;
                romFile.FileName = Path.GetFileName(sourceFile);
                return new List<RomFile>{romFile};
            }

            return GetContainingRomFiles(romFile);
        }

        private List<RomFile> GetContainingRomFiles(RomFile romFile)
        {
            var romFiles = new List<RomFile>();
            using (var archive = GetArchiveReader(romFile.CompressionType, romFile.SourceFile))
            {
                var files = archive.Entries.ToList();
                foreach (var file in files)
                {
                    var newRomFile = new RomFile
                    {
                        CompressionType = romFile.CompressionType,
                        SourceFile = romFile.SourceFile,
                        SourceFileName = Path.GetFileName(romFile.SourceFile),
                        System = romFile.System,
                        FileName = file.Key,
                        IsInCompressedFile = true
                    };

                    romFiles.Add(newRomFile);
                }
            }

            return romFiles;
        }

        private IArchive GetArchiveReader(CompressionType romFileCompressionType, string sourceFile)
        {
            switch (romFileCompressionType)
            {
                    case CompressionType.Sevenzip:
                        return SevenZipArchive.Open(sourceFile);
                    case CompressionType.Gzip:
                        return GZipArchive.Open(sourceFile);
                    case CompressionType.Rar:
                        return RarArchive.Open(sourceFile);
                    case CompressionType.Tar:
                        return TarArchive.Open(sourceFile);
                    case CompressionType.Zip:
                        return ZipArchive.Open(sourceFile);
            }

            return null;
        }

        private CompressionType GetCompressionType(string sourceFile)
        {
            if (SevenZipArchive.IsSevenZipFile(sourceFile))
                return CompressionType.Sevenzip;

            if (ZipArchive.IsZipFile(sourceFile))
                return CompressionType.Zip;

            if (GZipArchive.IsGZipFile(sourceFile))
                return CompressionType.Gzip;

            if (RarArchive.IsRarFile(sourceFile))
                return CompressionType.Rar;

            if(TarArchive.IsTarFile(sourceFile))
                return CompressionType.Tar;

            return CompressionType.None;
        }
    }
}
