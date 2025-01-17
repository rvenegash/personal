using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dtv.util.secure;

namespace testJarDll
{
    class Program
    {
        static void Main(string[] args)
        {
            var encriptado = Encryptor.process("504906444333339");


            var fecha = Encryptor.process("0822");
            //var x = new Encryptor();
            //var hash = x.pro();
            //int hash = x.GetHashCode();
        }
    }
}
