using System;
using System.IO;
using System.Collections.Generic;
namespace lab5_1
{
    class Task2
    {
        public static void ScanDisks()
        {
            var docs = new List<string>();

            foreach (var d in DriveInfo.GetDrives())
            {
                if (d.IsReady)
                {
                    try
                    {
                        docs.AddRange(Directory.EnumerateFiles(d.RootDirectory.FullName, "*.*", SearchOption.AllDirectories));
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Пропускаем папки без доступа
                    }
                    catch (DirectoryNotFoundException)
                    {
                        // Пропускаем несуществующие директории
                    }
                    catch (IOException)
                    {
                        // Пропускаем ошибки ввода-вывода
                    }
                }
            }

            try
            {
                File.WriteAllLines("docs.txt", docs);
            }
            catch (UnauthorizedAccessException)
            {
                // Ошибка записи в файл
            }
            catch (IOException)
            {
                // Ошибка ввода-вывода при записи
            }
        }
    }
}
