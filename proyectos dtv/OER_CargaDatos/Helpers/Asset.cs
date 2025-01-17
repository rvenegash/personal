using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OER_CargaDatos.Helpers
{
    public class Asset
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long TypeId { get; set; }
        public string NameFull { get; set; }
    }
}
