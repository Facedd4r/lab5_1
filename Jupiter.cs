using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5_1
{
    internal sealed class Jupiter : PlanetBase
    {
        private static readonly Lazy<Jupiter> _instance = new Lazy<Jupiter>(() => new Jupiter());
        public static Jupiter Instance => _instance.Value;

        private Jupiter() : base("Юпитер", 1.8982e27, 139820, 7.785e8) { }
    }
}
