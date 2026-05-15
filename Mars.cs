using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5_1
{
    internal sealed class Mars : PlanetBase
    {
        private static readonly Lazy<Mars> _instance = new Lazy<Mars>(() => new Mars());
        public static Mars Instance => _instance.Value;

        private Mars() : base("Марс", 6.4171e23, 6779, 2.279e8) { }
    }
}
