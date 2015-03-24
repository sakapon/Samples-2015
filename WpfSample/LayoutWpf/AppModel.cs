using System;
using System.Collections.Generic;
using System.Linq;

namespace LayoutWpf
{
    public class AppModel
    {
        public int[] Numbers { get; private set; }

        public AppModel()
        {
            Numbers = Enumerable.Range(1, 15).ToArray();
        }
    }
}
