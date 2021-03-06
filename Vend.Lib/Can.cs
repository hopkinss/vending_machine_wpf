using System;
using System.Collections.Generic;
using System.Text;


namespace Vend.Lib
{
    public class Can
    {
        private Flavor flavor;

        public Can(Flavor f)
        {
            this.flavor = f;
        }

        public Flavor Flavor
        {
            get { return flavor; }
            set { flavor = value; }
        }
    }
}
