using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5_1
{
    internal sealed class Earth : PlanetBase
    {
        private static readonly Lazy<Earth> _instance = new Lazy<Earth>(() => new Earth());
        public static Earth Instance => _instance.Value;

        private Earth(): base("Земля", 5.97237e24, 12742, 1.496e8) { }
    }
}
