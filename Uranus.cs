using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5_1
{
    internal sealed class Uranus : PlanetBase
    {
        private static readonly Lazy<Uranus> _instance = new Lazy<Uranus>(() => new Uranus());
        public static Uranus Instance => _instance.Value;

        private Uranus() : base("Уран", 8.6810e25, 50724, 2.871e9) { }
    }
}
