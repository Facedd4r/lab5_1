using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5_1
{
    internal sealed class Mercury : PlanetBase
    {
        private static readonly Lazy<Mercury> _instance = new Lazy<Mercury>(() => new Mercury());
        public static Mercury Instance => _instance.Value;

        // Счётчик вызовов конструктора(для проверки)
        public static int InstanceCount = 0;

        private Mercury() : base("Меркурий", 3.3011e23, 4879, 5.79e7)
        {
            // Увеличиваем счётчик при каждом создании объекта (должно быть 1)
            Interlocked.Increment(ref InstanceCount);
        }
    }
}
