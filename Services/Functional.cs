namespace Utilities.Functional{
    public class AES{
        private readonly string _keyS;

        public string KeyS { get; set; }
        public string ToBase2(int k)
        {
            string t = "";
            while (k != 0)
            {
                t += (k % 2);
                k /= 2;
            }
            return t;
        }
        public long PowerModEuclid(int power, int k, int modulo)
        {
            string d = ToBase2(k);
            int n = d.Length;
            long c = 1;
            long x;
            for (int i = n - 1; i >= 0; i--)
            {
                x = (d[i] == '1') ? power : 1;
                c = ((c * c) % modulo) * x;  // out of bound
                c %= modulo;
            }
            return c;
        }

    }
}