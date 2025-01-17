using System;
using System.Collections.Generic;
using System.Text;

namespace ParseaConfigJsonJenkinsK2v.Model
{
    class config
    {
        public string APP { get; set; }
        public string WS { get; set; }
        public environment[] ENVIRONMENTS { get; set; }
    }

    class environment
    {
        public string NAME { get; set; }
        public string FABRIC_SERVER { get; set; }
        public placeholder[] PLACEHOLDERS { get; set; }

    }

    class placeholder
    {
        public string PATH { get; set; }
        public placeholderitem[] PH { get; set; }

    }
    class placeholderitem
    {
        public string NOMBRE { get; set; }
        public string VALOR { get; set; }

    }
}
