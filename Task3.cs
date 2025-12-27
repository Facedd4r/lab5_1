using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace lab5_1
{
    public class Task3
    {
        public static void ExecuteBackupProcess()
        {
            string sourceDirectory = @"B:\test";
            string backupRootDirectory = @"B:\BackupFolder";

            if (!ValidateSourceDirectory(sourceDirectory))
                return;

            EnsureBackupRootExists(backupRootDirectory);

            int nextVersionNumber = DetermineNextVersionNumber(backupRootDirectory);

            bool backupNeeded = ShouldCreateBackup(sourceDirectory, backupRootDirectory, nextVersionNumber);

            if (backupNeeded)
            {
                CreateBackupWithSubdirectories(sourceDirectory, backupRootDirectory, nextVersionNumber);
                Console.WriteLine($"Создана резервная копия версии {nextVersionNumber} с сохранением структуры папок");
            }
            else
            {
                Console.WriteLine("Изменений не обнаружено, резервное копирование не требуется.");
            }
        }
        /// <summary>
        /// Метод проверки директории
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        private static bool ValidateSourceDirectory(string sourcePath)
        {
            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine($"Ошибка: исходная директория '{sourcePath}' не найдена.");
                return false;
            }
            return true;
        }

        private static void EnsureBackupRootExists(string backupPath)
        {
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }
        }

        private static int DetermineNextVersionNumber(string backupRoot)
        {
            if (!Directory.Exists(backupRoot))
                return 1;

            var versionFolders = Directory.GetDirectories(backupRoot)
                .Select(folder => Path.GetFileName(folder))
                .Where(name => int.TryParse(name, out _))
                .Select(name => int.Parse(name));

            return versionFolders.Any() ? versionFolders.Max() + 1 : 1;
        }

        private static bool ShouldCreateBackup(string sourcePath, string backupRoot, int nextVersion)
        {
            if (nextVersion == 1)
                return true;

            string previousVersionPath = Path.Combine(backupRoot, (nextVersion - 1).ToString());

            if (!Directory.Exists(previousVersionPath))
                return true;

            return AreDirectoriesDifferent(sourcePath, previousVersionPath);
        }
        /// <summary>
        /// Метод сравнения файлов
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="backupDir"></param>
        /// <returns></returns>
        private static bool AreDirectoriesDifferent(string sourceDir, string backupDir)
        {
            var sourceFiles = GetAllFilesWithRelativePaths(sourceDir);
            var backupFiles = GetAllFilesWithRelativePaths(backupDir);

            if (sourceFiles.Count != backupFiles.Count)
                return true;

            using var md5 = MD5.Create();

            foreach (var sourceFile in sourceFiles)
            {
                string relativePath = sourceFile.Key;

                if (!backupFiles.ContainsKey(relativePath))
                    return true;

                string sourceFullPath = Path.Combine(sourceDir, relativePath);
                string backupFullPath = Path.Combine(backupDir, relativePath);

                if (!CompareFilesByHash(sourceFullPath, backupFullPath, md5))
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Новый метод для получения файлов с путями
        /// </summary>
        /// <param name="rootDirectory"></param>
        /// <returns></returns>
        private static Dictionary<string, FileInfo> GetAllFilesWithRelativePaths(string rootDirectory)
        {
            var fileDictionary = new Dictionary<string, FileInfo>();

            if (!Directory.Exists(rootDirectory))
                return fileDictionary;

            string[] allFiles = Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories);

            foreach (string filePath in allFiles)
            {
                string relativePath = GetRelativePath(rootDirectory, filePath);
                var fileInfo = new FileInfo(filePath);
                fileDictionary[relativePath] = fileInfo;
            }

            return fileDictionary;
        }

        private static string GetRelativePath(string rootPath, string fullPath)
        {
            Uri rootUri = new Uri(rootPath + Path.DirectorySeparatorChar);
            Uri fileUri = new Uri(fullPath);
            return Uri.UnescapeDataString(rootUri.MakeRelativeUri(fileUri).ToString());
        }

        private static bool CompareFilesByHash(string filePath1, string filePath2, MD5 hasher)
        {
            try
            {
                if (!File.Exists(filePath1) || !File.Exists(filePath2))
                    return false;

                string hash1 = CalculateFileHash(filePath1, hasher);
                string hash2 = CalculateFileHash(filePath2, hasher);

                return hash1 == hash2;
            }
            catch
            {
                return false;
            }
        }

        private static string CalculateFileHash(string filePath, MD5 hasher)
        {
            using var fileStream = File.OpenRead(filePath);
            byte[] hashBytes = hasher.ComputeHash(fileStream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
        /// <summary>
        /// Метод создания бэкапа
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="backupRoot"></param>
        /// <param name="version"></param>
        private static void CreateBackupWithSubdirectories(string sourceDir, string backupRoot, int version)
        {
            string versionDirectory = Path.Combine(backupRoot, version.ToString());

            try
            {
                Console.WriteLine($"Начало создания резервной копии версии {version}...");

                int copiedFiles = CopyDirectoryRecursive(sourceDir, versionDirectory);

                Console.WriteLine($"Резервная копия создана успешно. Скопировано файлов: {copiedFiles}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании резервной копии: {ex.Message}");

                // Попытка удалить частично созданную папку при ошибке
                try
                {
                    if (Directory.Exists(versionDirectory))
                    {
                        Directory.Delete(versionDirectory, true);
                    }
                }
                catch
                {
                    // Игнорируем ошибки при удалении
                }

                throw;
            }
        }

        private static int CopyDirectoryRecursive(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(sourceDir))
                return 0;

            Directory.CreateDirectory(targetDir);
            int fileCount = 0;

            // Копируем все файлы из текущей директории
            foreach (string sourceFile in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(sourceFile);
                string targetFile = Path.Combine(targetDir, fileName);
                File.Copy(sourceFile, targetFile, true);
                fileCount++;
            }

            // Рекурсивно копируем поддиректории
            foreach (string sourceSubDir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(sourceSubDir);
                string targetSubDir = Path.Combine(targetDir, dirName);
                fileCount += CopyDirectoryRecursive(sourceSubDir, targetSubDir);
            }

            return fileCount;
        }
    }
}