using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillerAPI
{
    public class MenuOption
    {
        public string Route { get; set; }
        public string Name { get; set; }
        public MenuOption(string route, string name) // i don't love putting the routes in c#. 
        {
            // it really seems like the routes should be long in the js. hm.
            Route = route;
            Name = name;
        }
    }
}
