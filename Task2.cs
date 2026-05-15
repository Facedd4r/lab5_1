using System;
using System.IO;
using System.Collections.Generic;
namespace lab5_1
{
    class Task2
    {
        // 1. Поиск в каждом диске в отдельном потоке
        public static void ScanDisksParallelByDrive()
        {
            var allFiles = new List<string>();
            var lockObj = new object();
            var threads = new List<Thread>();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady) continue;
                if (drive.Name == @"C:\") continue; // пропускаем диск C

                Thread thread = new Thread(() =>
                {
                    ScanDrive(drive, allFiles, lockObj);
                });
                thread.Start();
                threads.Add(thread);
            }

            // Ожидаем завершения всех потоков
            foreach (var t in threads)
                t.Join(); // приостанавливает текущий поток (главный) до тех пор, пока поток t не завершится.

            // Записываем результат в файл
            try
            {
                File.WriteAllLines("docs_parallel_drives.txt", allFiles);
                Console.WriteLine($"Сканирование завершено. Найдено файлов: {allFiles.Count}. Результат сохранён в docs_parallel_drives.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка записи результата: {ex.Message}");
            }
        }

        private static void ScanDrive(DriveInfo drive, List<string> resultList, object locker)
        {
            string root = drive.RootDirectory.FullName;
            Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} начал сканирование диска {root}");

            ScanDirectoryRecursive(root, resultList, locker);

            Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} закончил сканирование диска {root}");
        }
        private static void ScanSingleFolder(string folder, List<string> resultList, object locker)
        {
            Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} начал обход папки {folder}");
            ScanDirectoryRecursive(folder, resultList, locker);
            Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} закончил обход папки {folder}");
        }

        // 2. Поиск в каждой папке корневой директории в отдельном потоке
        public static void ScanDisksParallelByRootFolders(string rootDirectory = null)
        {
            // Если директория не указана – используем текущую или корневую диска C:
            if (string.IsNullOrEmpty(rootDirectory))
                rootDirectory = @"C:\";

            if (!Directory.Exists(rootDirectory))
            {
                Console.WriteLine($"Директория {rootDirectory} не существует.");
                return;
            }

            var allFiles = new List<string>();
            var lockObj = new object();
            var threads = new List<Thread>();

            // Получаем все папки первого уровня в корневой директории
            string[] subFolders;
            try
            {
                subFolders = Directory.GetDirectories(rootDirectory);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Нет доступа к корневой папке {rootDirectory}");
                return;
            }

            if (subFolders.Length == 0)
            {
                Console.WriteLine($"В директории {rootDirectory} нет подпапок для сканирования.");
                return;
            }

            foreach (string folder in subFolders)
            {
                Thread thread = new Thread(() =>
                {
                    ScanSingleFolder(folder, allFiles, lockObj);
                });
                thread.Start();
                threads.Add(thread);
            }

            // Ожидаем завершения всех потоков
            foreach (var t in threads)
                t.Join();

            // Записываем результат в файл
            try
            {
                File.WriteAllLines("docs_parallel_folders.txt", allFiles);
                Console.WriteLine($"Сканирование завершено. Найдено файлов: {allFiles.Count}. Результат сохранён в docs_parallel_folders.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка записи результата: {ex.Message}");
            }
        }

        // Рекурсивный обход директории с добавлением путей файлов в общий список (потокобезопасно)
        private static void ScanDirectoryRecursive(string directory, List<string> resultList, object locker)
        {
            // Обрабатываем файлы в текущей папке
            try
            {
                foreach (string file in Directory.GetFiles(directory))
                {
                    lock (locker)
                    {
                        resultList.Add(file);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Нет доступа – пропускаем папку
                return;
            }
            catch (DirectoryNotFoundException)
            {
                // Папка могла быть удалена во время сканирования
                return;
            }
            catch (IOException)
            {
                // Другие ошибки ввода-вывода
                return;
            }

            // Рекурсивно обходим подпапки
            try
            {
                foreach (string subDir in Directory.GetDirectories(directory))
                {
                    ScanDirectoryRecursive(subDir, resultList, locker);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Нет доступа к подпапкам
                return;
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }
            catch (IOException)
            {
                return;
            }
        }
    }
}
