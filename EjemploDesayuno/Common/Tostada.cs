namespace Common
{
    public class Tostada
    {
        private int Cantidad {  get; set; }
        private bool _tieneManteca;
        private bool _tieneMermelada;

        public bool TieneMermelada
        {
            get => _tieneMermelada;
            set => _tieneMermelada = value;
        }

        public bool TieneManteca
        {
            get => _tieneMermelada;
            set => _tieneMermelada = value;
        }

        public Tostada(int cantidad)
        {
            Cantidad = cantidad;
        }
    }
}
