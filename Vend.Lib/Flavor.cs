using System;
using System.Collections.Generic;
using System.Linq;

namespace Vend.Lib
{
    public enum Flavor { Regular = 1, Orange, Lemon }

    public static class FlavorOps
    {
        private static List<Flavor> allFlavors = new List<Flavor>();

        static FlavorOps()
        {
            allFlavors.AddRange(Enum.GetValues(typeof(Flavor)).Cast<Flavor>().ToList());
        }

        public static List<Flavor> AllFlavors
        {
            get { return allFlavors; }
        }

        public static Flavor ToFlavor(string FlavorName)
        {
            if (Enum.TryParse<Flavor>(FlavorName, true, out Flavor flavor))
            {
                if (Enum.IsDefined(typeof(Flavor), flavor))
                {
                    return flavor;
                }
            }

            return default;
        }
    }
}
