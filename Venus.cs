using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5_1
{
    internal sealed class Venus : PlanetBase
    {
        private static readonly Lazy<Venus> _instance = new Lazy<Venus>(() => new Venus());
        public static Venus Instance => _instance.Value;

        private Venus(): base("Венера", 4.8675e24, 12104, 1.082e8) { }
    }
}
