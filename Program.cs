
using lab5_1;

public class Program
{
    public static void Main()
    {
        // Параллельное сканирование всех дисков,каждый диск в отдельном потоке
        //Task2.ScanDisksParallelByDrive();

        // Параллельное сканирование подпапок корневой директории (B:\)
        //Task2.ScanDisksParallelByRootFolders(@"B:\");

        // солнечная система с singleton
        var planets = new IPlanet[]
        {
                Mercury.Instance,
                Venus.Instance,
                Earth.Instance,
                Mars.Instance,
                Jupiter.Instance,
                Saturn.Instance,
                Uranus.Instance,
                Neptune.Instance
        };

        foreach (var planet in planets)
        {
            planet.ShowInfo();
            Console.WriteLine();
        }
        // ПРОВЕРКА потокобезопасности на примере Меркурия
        var references = new List<Mercury>();
        var threads = new List<Thread>();

        for (int i = 0; i < 20; i++)
        {
            var thread = new Thread(() =>
            {
                var inst = Mercury.Instance; // получаем экземпляр Mercury
                lock (references) // синхронизируем доступ к списку
                {
                    references.Add(inst);
                }
            });
            threads.Add(thread);
            thread.Start();
        }

        foreach (var t in threads) t.Join(); // ждём завершения всех потоков

        // возвращает коллекцию уникальных ссылок на объекты Mercury
        // если синглтон работает правильно, все 20 ссылок будут указывать на один и тот же объект, поэтому Distinct().Count() вернёт 1
        Console.WriteLine($"Уникальных экземпляров: {references.Distinct().Count()}"); // должно быть 1
    }
}