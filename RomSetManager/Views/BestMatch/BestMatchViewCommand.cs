using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Threading;
using Caliburn.Micro;
using Microsoft.Win32;
using Model;
using RomSetManager.Views.Dialogs;
using RomSetManager.Views.Dialogs.BestMatchFilter;
using RomSetManager.Worker;
using Configuration = Model.Configuration;
using MessageBox = System.Windows.Forms.MessageBox;

namespace RomSetManager.Views.BestMatch
{
    public partial class BestMatchViewModel
    {
        public void EditPreferences()
        {
            var dialogViewModel = Container.GetInstance<BestMatchFilterDialogViewModel>();

            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.ResizeMode = ResizeMode.CanResizeWithGrip;
            settings.MinWidth = 600;
            settings.Title = "Select your Preferences";
            //settings.Icon = new BitmapImage(new Uri("pack://application:,,,/MyApplication;component/Assets/myicon.ico"));


            IWindowManager manager = new WindowManager();
            manager.ShowDialog(dialogViewModel, null, settings);
        }

        public void SelectSourceDirectory()
        {
            SourceDirectory = OpenFolderChooseDialog(SourceDirectory);
            var config = _configurationService.GetConfiguration();
            config.BestMatch.RomSourceDirectory = SourceDirectory;
            _configurationService.UpdateConfiguration(config);
        }

        public void SelectDestinationDirectory()
        {
            DestinationDirectory = OpenFolderChooseDialog(DestinationDirectory);
            var config = _configurationService.GetConfiguration();
            config.BestMatch.RomDestinationDirectory = DestinationDirectory;
            _configurationService.UpdateConfiguration(config);
        }



        /// <summary>
        /// clears the grid and adds the given romfiles
        /// </summary>
        /// <param name="romFiles"></param>
        private void SetRomList(List<RomFile> romFiles)
        {
            RomFiles.Clear();

            foreach (var romFile in romFiles)
            {
                RomFiles.Add(romFile);
            }
        }

        public void CloneDirectoryFromRetropie()
        {
            var tmpDir = "";

            while (true)
            {
                tmpDir = OpenFolderChooseDialog(SourceDirectory, "Select a directory where the retropie-folder-structure gets copied to.");
                if (string.IsNullOrEmpty(tmpDir))
                    return;

                if (!Directory.EnumerateFileSystemEntries(tmpDir).Any())
                    break;

                MessageBox.Show("The directory has to be empty.");
            }

            SourceDirectory = tmpDir;

            var config = _configurationService.GetConfiguration();
            config.BestMatch.RomSourceDirectory = SourceDirectory;
            _configurationService.UpdateConfiguration(config);


            var retropieDir = @"\\RETROPIE\roms";
            
            var target = new DirectoryInfo(SourceDirectory);
            foreach (var directory in new DirectoryInfo(retropieDir).GetDirectories())
            {
                target.CreateSubdirectory(directory.Name);
            }
            
        }

        #region 3. Actions

        public void ReadSourceRomFiles()
        {
            var config = _configurationService.GetConfiguration();
            var romFileWorker = new RomFileWorker(config);

            romFileWorker.BackgroundWorker.ProgressChanged += (sender, args) =>
            {
                LoadScreenProgress = args.ProgressPercentage;
            };
            romFileWorker.BackgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                var romFiles = (List<RomFile>) args.Result;
                SetRomList(romFiles);

                NotifyOfPropertyChange(() => CanWipeFileNames);
                HideLoadScreen();
            };

            ShowLoadScreen("Reading Rom-Files...");
            romFileWorker.GetRomFiles();
        }

        public bool CanWipeFileNames => RomFiles.Count > 0;
        public void WipeFileNames()
        {
            var config = _configurationService.GetConfiguration();
            var romFileWorker = new RomFileWorker(config);

            romFileWorker.BackgroundWorker.ProgressChanged += (sender, args) =>
            {
                LoadScreenProgress = args.ProgressPercentage;
            };
            romFileWorker.BackgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                var romFiles = (List<RomFile>) args.Result;
                SetRomList(romFiles);
                NotifyOfPropertyChange(() => CanExport);
                HideLoadScreen();
            };

            ShowLoadScreen("Wiping Rom-Names...");
            romFileWorker.WipeFileNames(RomFiles.ToList());
                
        }

        public bool CanExport => RomFiles.Any(r => r.Export);
        public void Export()
        {
            var config = _configurationService.GetConfiguration();
            var romFileWorker = new RomFileWorker(config);

            romFileWorker.BackgroundWorker.ProgressChanged += (sender, args) =>
            {
                LoadScreenProgress = args.ProgressPercentage;
            };
            romFileWorker.BackgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                HideLoadScreen();
            };

            ShowLoadScreen("Exporting wiped roms to ["+DestinationDirectory+"]...");
            romFileWorker.Export(RomFiles.Where(r => r.Export).ToList());
        }

        #endregion
        //private void DeepCopy(DirectoryInfo source, DirectoryInfo target)
        //{

        //    // Recursively call the DeepCopy Method for each Directory
        //    foreach (DirectoryInfo dir in source.GetDirectories())
        //        DeepCopy(dir, target.CreateSubdirectory(dir.Name));

        //    // Go ahead and copy each file in "source" to the "target" directory
        //    foreach (FileInfo file in source.GetFiles())
        //        file.CopyTo(Path.Combine(target.FullName, file.Name));

        //}

        private string OpenFolderChooseDialog(string directory, string description = null)
        {
            var fd = new FolderBrowserDialog();
            if (!string.IsNullOrEmpty(directory))
                fd.SelectedPath = directory;

            fd.Description = description;

            var result = fd.ShowDialog();
            if (result != DialogResult.OK)
                return null;

            return fd.SelectedPath;
        }
    }
}