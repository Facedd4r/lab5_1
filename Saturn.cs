using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5_1
{
    internal sealed class Saturn : PlanetBase
    {
        private static readonly Lazy<Saturn> _instance = new Lazy<Saturn>(() => new Saturn());
        public static Saturn Instance => _instance.Value;

        private Saturn(): base("Сатурн", 5.6834e26, 116460, 1.434e9) { }
    }
}
