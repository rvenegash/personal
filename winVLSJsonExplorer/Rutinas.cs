namespace winVLSJsonExplorer
{
    internal class Rutinas
    {
        private List<Pais> _paises;

        public Rutinas()
        {
            _paises = new List<Pais>();
            _paises.Add(new Pais { pais = "AR", cssId = 4 });
            _paises.Add(new Pais { pais = "PE", cssId = 5 });
            _paises.Add(new Pais { pais = "CO", cssId = 7 });
            _paises.Add(new Pais { pais = "CA", cssId = 9 });
            _paises.Add(new Pais { pais = "CL", cssId = 12 });
            _paises.Add(new Pais { pais = "UY", cssId = 14 });
            _paises.Add(new Pais { pais = "EC", cssId = 15 });
        }

        internal List<string> getPaises(int Css)
        {
            List<Pais> _paisesEnc = new List<Pais>();

            int number = Css; // Binary: 11111111, Hex: 0xFF
            foreach (var pais in _paises)
            {
                int bitPosition = pais.cssId; // Check bit at position 3 (0-indexed from right)
                                              // bool isBitSet = (number & (1 << bitPosition)) != 0;

                // Console.WriteLine($"Bit at position {bitPosition} is: {(isBitSet ? "1" : "0")}");

                bool isBitSet2 = System.Numerics.BitOperations.IsPow2(number & (1 << bitPosition));
                if (isBitSet2)
                {
                    _paisesEnc.Add(pais);
                }
            }

            return _paisesEnc.Select(m => m.pais).ToList();
        }
    }

    public class Pais
    {
        public string pais { get; set; }
        internal int cssId { get; set; }  //base 0
    }
}
