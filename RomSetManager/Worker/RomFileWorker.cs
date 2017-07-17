using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Model;
using Model.Constants;
using SharpCompress.Archives;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;
using SharpCompress.Readers;

namespace RomSetManager.Worker
{
    public class RomFileWorker
    {
        public Model.Configuration Configuration { get; set; }
        private string _sourceDirectory;
        private string _destinationDirectory;

        public RomFileWorker(Model.Configuration configuration)
        {
            Configuration = configuration;
            _sourceDirectory = configuration.BestMatch.RomSourceDirectory;
            _destinationDirectory = configuration.BestMatch.RomDestinationDirectory;
        }

        public List<RomFile> GetRomFiles()
        {
            var files = Directory.GetFiles(_sourceDirectory, "*.*", SearchOption.AllDirectories);
            var ext = new List<string>{"log","txt"};

            var romFiles = new List<RomFile>();
            foreach (var file in files.Where(f => ext.All(e => !f.ToLower().EndsWith(e.ToLower()))))
            {
                var tmpRomFiles = Read(file);
                romFiles.AddRange(tmpRomFiles);
            }

            return romFiles;
        }

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

            if (romFile.CompressionType == CompressionType.None || KeepCompressed(romFile.System)) //no compression or file should not be decompressed.
            {
                romFile.IsInCompressedFile = false;
                romFile.FileName = Path.GetFileName(sourceFile);
                return new List<RomFile> { romFile };
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
                        IsInCompressedFile = true,
                        FileNameWiped = ""
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

            if (TarArchive.IsTarFile(sourceFile))
                return CompressionType.Tar;

            return CompressionType.None;
        }

        /// <summary>
        /// Checks if the given system needs the rom-files compressed.
        /// If true, we don't need to open the compressed file because all files in there are needed by the system.
        /// </summary>
        /// <param name="system"></param>
        /// <returns></returns>
        private bool KeepCompressed(string system)
        {
            var configSystem = Configuration.Systems.FirstOrDefault(s => s.Name == system);
            return configSystem != null && configSystem.KeepCompressed;
        }


        //private bool IsNameWipingNeeded(string fileName)
        //{
        //    var parts = GetWipeParts(fileName);
        //    return parts.Count > 0;
        //}

        private bool IsValidSystem(string system, string configSystem)
        {
            if (configSystem == "any")
                return true;

            var systems = configSystem.ToLower().Split(',').ToList(); //f.e. genesis,megadrive
          
            return systems.Any(s => s == system.ToLower());

        }

        private List<string> GetWipeParts(string system, string fileName, List<NamePart> nameParts)
        {
            var list = new List<string>();
           
            foreach (var part in nameParts.Where(n => IsValidSystem(system, n.System)))
            {
                var parts = part.Value.Split(','); //split if multiple values
                foreach (var tmpPart in parts.Select(p => p.Trim()))
                {
                    if (tmpPart.Contains("*")) //need to use regex to compare
                    {
                        var wildcardPattern = tmpPart.Replace("(", @"\(").Replace(")", @"\)").Replace("[", @"\[").Replace("]", @"\]").Replace("*", ".*").Replace("+",@"\+").Replace("-", @"\-");
                       
                        foreach (Match match in Regex.Matches(fileName, wildcardPattern))
                        {
                            list.Add(match.Value);
                        }

                    }
                    else if (tmpPart.Contains("#"))
                    {
                        var wildcardPattern = tmpPart.Replace("(", @"\(").Replace(")", @"\)").Replace("[", @"\[").Replace("]", @"\]").Replace("*", ".*").Replace("+", @"\+").Replace("-", @"\-")
                            .Replace("#", @"\d");//#=digit
                        foreach (Match match in Regex.Matches(fileName, wildcardPattern))
                        {
                            list.Add(match.Value);
                        }
                    }
                    else if (fileName.Contains(tmpPart))
                        list.Add(tmpPart);
                }
            }

            return list.Distinct().OrderByDescending(l => l.Length).ToList(); //order by length is just a workaround because the regex above has some problems when mutliple brakets are present (results has multiple-values)
        }

        /// <summary>
        /// will get called to find out if there are still some braket-values in the filename. this could be a potential missing configuration.
        /// </summary>
        /// <returns></returns>
        private int NumberOfBraketValues(string wipedName)
        {
            var pattern = @"(\[.*\]|\(.*\))";
            return Regex.Matches(wipedName, pattern).Count;
        }

        public List<RomFile> WipeFileNames(List<RomFile> romFiles)
        {
            foreach (var romFile in romFiles)
            {
                romFile.Export = false; //reset to false because it will set to true below (if match)
                var parts = GetWipeParts(romFile.System, romFile.FileName, Configuration.BestMatch.Preferences.NameParts);
                var wipedName = romFile.FileName;

                if (parts.Count >0)
                {
                    foreach (var part in parts.Where(p => p.Length > 0))
                    {
                        wipedName = wipedName.Replace(part, "");
                    }
                }

                wipedName = Regex.Replace(wipedName, @"\s+", " "); //replacing multiple whitespaces
                wipedName = wipedName.Replace(" .", ".");//in case there is an space before the dot.
                romFile.FileNameWiped = wipedName;
                romFile.WipedPartsCounter = parts.Count;
                romFile.NotWipedPartsCounter = NumberOfBraketValues(wipedName);
            }


            FilterByPreferences(romFiles);

            return romFiles;
        }

        private void FilterByPreferences(List<RomFile> romFiles)
        {
            var nameParts = Configuration.BestMatch.Preferences.NameParts;
            var favorite = nameParts.Where(n => n.Behaviour == BehaviourType.Favorite).OrderBy(n => n.Position).ToList();
            var mustHaves = nameParts.Where(n => n.Behaviour == BehaviourType.MustHave).ToList();
            var neverUse = nameParts.Where(n => n.Behaviour == BehaviourType.NeverUse).ToList();
            var dontCare = nameParts.Where(n => n.Behaviour == BehaviourType.DontCare).ToList();

            var filtered = new List<RomFile>();

            foreach (var romFile in romFiles)
            {
                if (mustHaves.Count > 0 && !GetWipeParts(romFile.System, romFile.FileName, mustHaves).Any()) //no mustHave-part found? continue with next rom.
                    continue;

                if (neverUse.Count > 0 && GetWipeParts(romFile.System, romFile.FileName, neverUse).Any()) //any neveruse-part found? continue with next rom.
                    continue;

                filtered.Add(romFile);
            }

            var result = new List<RomFile>();

            var fileNames = filtered.Select(f => Path.GetFileNameWithoutExtension(f.FileNameWiped)).Distinct().OrderBy(f => f).ToList();
            foreach (var fileName in fileNames)
            {
                var roms = filtered.Where(f => Path.GetFileNameWithoutExtension(f.FileNameWiped) == fileName).ToList();

                var prevRoms = roms.ToList();
                if (roms.Count > 1)
                {
                    foreach (var namePart in favorite)
                    {
                        roms.RemoveAll(r => !GetWipeParts(r.System, r.FileName, new List<NamePart> {namePart}).Any());
                        if (roms.Count ==0)
                            break;

                        prevRoms = roms.ToList();
                        if (roms.Count == 1)
                            break;
                    }
                }
               

                var winner = prevRoms.First();
                winner.Export = true;
            }
        }



        public void Export(List<RomFile> romFiles)
        {
            var outDir = Configuration.BestMatch.RomDestinationDirectory;

            foreach (var romFile in romFiles)
            {
                try
                {
                    var dir = Path.Combine(outDir, romFile.System);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var destFile = Path.Combine(dir, romFile.FileNameWiped);
                    var configSystem = Configuration.Systems.First(s => s.Name == romFile.System);

                    if (romFile.IsInCompressedFile && configSystem.KeepCompressed==false)
                    {
                        var compressionType = GetCompressionType(romFile.SourceFile);
                        var archiveReader = GetArchiveReader(compressionType, romFile.SourceFile);

                        var file = archiveReader.Entries.FirstOrDefault(e => e.Key == romFile.FileName);
                        if (file == null)
                            continue;

                        file.WriteToFile(destFile,new ExtractionOptions{Overwrite = true});

                    }
                    else
                    {
                        File.Copy(romFile.SourceFile, destFile);
                    }
                }
                catch (Exception exception)
                {
                    
                }

                
            }
        }
    }
}
