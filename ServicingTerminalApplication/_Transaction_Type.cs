using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicingTerminalApplication
{
    public class _Transaction_Type
    {
        public int id { get; set; }
        public string Transaction_Name { get; set; }
        public string Description { get; set; }
        public string Short_Name { get; set; }
        public int Pattern_Max { get; set; } = 0;

    }

}
