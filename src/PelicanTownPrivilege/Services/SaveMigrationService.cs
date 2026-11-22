using System;
using System.IO;
using StardewModdingAPI;

namespace PelicanTownPrivilege.Services
{
    /// <summary>Makes recoverable copies of a save before the mod changes anything in it.</summary>
    public sealed class SaveMigrationService
    {
        private readonly Diagnostics diagnostics;

        public SaveMigrationService(Diagnostics diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        /// <summary>
        /// Copies the current save folder to PelicanTownPrivilege-backups, beside the Saves folder (never inside it).
        /// Returns the backup path, or null if the copy failed.
        /// </summary>
        public string CreateBackup(string reason)
        {
            try
            {
                string saves = Constants.SavesPath;
                string saveFolder = Path.Combine(saves, Constants.SaveFolderName);
                if (!Directory.Exists(saveFolder))
                {
                    diagnostics.Warn($"Backup skipped: save folder not found at {saveFolder}.");
                    return null;
                }

                string parent = Path.GetDirectoryName(Path.GetFullPath(saves)) ?? saves;
                string target = Path.Combine(parent, "PelicanTownPrivilege-backups",
                    $"{Constants.SaveFolderName}-{DateTime.Now:yyyyMMdd-HHmmss}-{reason}");
                CopyDirectory(saveFolder, target);
                diagnostics.Info($"Backup created: {target}");
                return target;
            }
            catch (Exception ex)
            {
                diagnostics.Error("Backup failed", ex);
                return null;
            }
        }

        private static void CopyDirectory(string source, string target)
        {
            Directory.CreateDirectory(target);
            foreach (string file in Directory.GetFiles(source))
                File.Copy(file, Path.Combine(target, Path.GetFileName(file)), overwrite: true);
            foreach (string directory in Directory.GetDirectories(source))
                CopyDirectory(directory, Path.Combine(target, Path.GetFileName(directory)));
        }
    }
}
