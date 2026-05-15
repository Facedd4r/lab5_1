using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5_1
{
    internal sealed class Neptune : PlanetBase
    {
        private static readonly Lazy<Neptune> _instance = new Lazy<Neptune>(() => new Neptune());
        public static Neptune Instance => _instance.Value;

        private Neptune() : base("Нептун", 1.02413e26, 49244, 4.495e9) { }
    }
}
