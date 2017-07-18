using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public BackgroundWorker BackgroundWorker;
        public Model.Configuration Configuration { get; set; }

        public RomFileWorker(Model.Configuration configuration)
        {
            Configuration = configuration;
            BackgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
        }

        private void NotifyProgress(int currentCounter, int totalCounter)
        {
            var progress = (int)(((double)currentCounter / (double)totalCounter) * 100);
            BackgroundWorker.ReportProgress(progress);
        }

        public void GetRomFiles()
        {
            BackgroundWorker.DoWork += (sender, args) =>
            {
                var files = Directory.GetFiles(Configuration.BestMatch.RomSourceDirectory, "*.*", SearchOption.AllDirectories);
                var ext = new List<string> { "log", "txt" };

                var romFiles = new List<RomFile>();
                var filteredFiles = files.Where(f => ext.All(e => !f.ToLower().EndsWith(e.ToLower()))).ToList();
                var totalCount = filteredFiles.Count;

                var fileCounter = 0;
                foreach (var file in filteredFiles)
                {
                    var tmpRomFiles = Read(file);
                    romFiles.AddRange(tmpRomFiles);
                    fileCounter++;

                    NotifyProgress(fileCounter, totalCount);
                }

                args.Result = romFiles;
            };

            BackgroundWorker.RunWorkerAsync();
            //return romFiles;
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

        public void WipeFileNames(List<RomFile> romFiles)
        {
            BackgroundWorker.DoWork += (sender, args) =>
            {
                var total = romFiles.Count*2; //*2 just because its more to do...just for visualization
                var currentCounter = 0;

                foreach (var romFile in romFiles)
                {
                    romFile.Export = false; //reset to false because it will set to true below (if match)
                    var parts = GetWipeParts(romFile.System, romFile.FileName, Configuration.BestMatch.Preferences.NameParts);
                    var wipedName = romFile.FileName;

                    if (parts.Count > 0)
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

                    currentCounter++;
                    NotifyProgress(currentCounter, total);
                }
                
                FilterByPreferences(romFiles);
                args.Result = romFiles;
            };
                
            BackgroundWorker.RunWorkerAsync();

            //return romFiles;
        }

        private void FilterByPreferences(List<RomFile> wipedRomFiles)
        {
            var nameParts = Configuration.BestMatch.Preferences.NameParts;
            var favorite = nameParts.Where(n => n.Behaviour == BehaviourType.Favorite).OrderBy(n => n.Position).ToList();
            var mustHaves = nameParts.Where(n => n.Behaviour == BehaviourType.MustHave).ToList();
            var neverUse = nameParts.Where(n => n.Behaviour == BehaviourType.NeverUse).ToList();
            var dontCare = nameParts.Where(n => n.Behaviour == BehaviourType.DontCare).ToList();

            var filtered = new List<RomFile>();

            //Grouping filenames before iterating through all rom files. this is much faster than commparing in loop.
            var groupedRomNames = wipedRomFiles.GroupBy(r => Path.GetFileNameWithoutExtension(r.FileNameWiped))
                .Select(grp => new {
                    Name = grp.Key,
                    Counter = grp.Count()
                })
                .ToList();

            foreach (var romFile in wipedRomFiles)
            {
                var romNameCount = groupedRomNames.First(g => g.Name == Path.GetFileNameWithoutExtension(romFile.FileNameWiped)).Counter;

                //get rom-count with identical names to current rom
                //var romNameCount = 2;//wipedRomFiles.Count(c => Path.GetFileNameWithoutExtension(c.FileNameWiped) ==Path.GetFileNameWithoutExtension(romFile.FileNameWiped));

                if ((romNameCount!=1 || !Configuration.BestMatch.Preferences.IgnoreMustHaveForOneRom) && //if option is set to true and only one rom is available for this game we ignore this check
                    mustHaves.Count > 0 && !GetWipeParts(romFile.System, romFile.FileName, mustHaves).Any()) //no mustHave-part found? continue with next rom.
                    continue;

                if ((romNameCount != 1 || !Configuration.BestMatch.Preferences.IgnoreNeverUseForOneRom) && //if option is set to true and only one rom is available for this game we ignore this check
                    neverUse.Count > 0 && GetWipeParts(romFile.System, romFile.FileName, neverUse).Any()) //any neveruse-part found? continue with next rom.
                    continue;

                filtered.Add(romFile);
            }
            
            var filteredFileNames = filtered.Select(f => Path.GetFileNameWithoutExtension(f.FileNameWiped)).Distinct().OrderBy(f => f).ToList();
            var counter = (wipedRomFiles.Count / 2); //because we know that all roms are init already. and we set the progressbar-total to rom.Count*2 we will reduce it here again.
            var total = counter + filteredFileNames.Count;
            
            foreach (var fileName in filteredFileNames)
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
                counter++;
                NotifyProgress(counter,total);
                var winner = prevRoms.First();
                winner.Export = true;
            }
        }



        public void Export(List<RomFile> romFiles)
        {
            BackgroundWorker.DoWork += (sender, args) =>
            {
                var outDir = Configuration.BestMatch.RomDestinationDirectory;

                var counter = 0;
                var total = romFiles.Count;

                foreach (var romFile in romFiles)
                {
                    try
                    {
                        counter++;
                        NotifyProgress(counter,total);

                        var dir = Path.Combine(outDir, romFile.System);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        var destFile = Path.Combine(dir, romFile.FileNameWiped);
                        var configSystem = Configuration.Systems.First(s => s.Name == romFile.System);

                        if (romFile.IsInCompressedFile && configSystem.KeepCompressed == false)
                        {
                            var compressionType = GetCompressionType(romFile.SourceFile);
                            var archiveReader = GetArchiveReader(compressionType, romFile.SourceFile);

                            var file = archiveReader.Entries.FirstOrDefault(e => e.Key == romFile.FileName);
                            if (file == null)
                                continue;

                            file.WriteToFile(destFile, new ExtractionOptions { Overwrite = true });

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
            };

            BackgroundWorker.RunWorkerAsync();
                
        }
    }
}
