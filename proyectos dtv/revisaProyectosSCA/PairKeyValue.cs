using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace revisaProyectosSCA
{
    class PairKeyValue
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }

        public PairKeyValue(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public PairKeyValue(long id, string name, string key)
        {
            this.Id = id;
            this.Name = name;
            this.Key = key;
        }
    }
}
