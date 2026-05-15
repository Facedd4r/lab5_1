using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5_1
{
    internal abstract class PlanetBase : IPlanet
    {
        public string Name { get; protected set; }
        public double MassKg { get; protected set; }
        public double DiameterKm { get; protected set; }
        public double DistanceFromSunKm { get; protected set; }

        protected PlanetBase(string name, double massKg, double diameterKm, double distanceFromSunKm)
        {
            Name = name;
            MassKg = massKg;
            DiameterKm = diameterKm;
            DistanceFromSunKm = distanceFromSunKm;
        }

        public virtual void ShowInfo()
        {
            Console.WriteLine($"Планета: {Name}");
            Console.WriteLine($"  Масса: {MassKg} кг");
            Console.WriteLine($"  Диаметр: {DiameterKm} км");
            Console.WriteLine($"  Расстояние от Солнца: {DistanceFromSunKm} км");
        }
    }
}
