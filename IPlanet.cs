using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5_1
{
    internal interface IPlanet
    {
        string Name { get; }
        double MassKg { get; }
        double DiameterKm { get; }
        double DistanceFromSunKm { get; }
        void ShowInfo();
    }
}
