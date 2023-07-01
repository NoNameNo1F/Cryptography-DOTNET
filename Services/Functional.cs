using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Utilities.Functional{
    public class MyAES{
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
    public class MyRSA{
        private static RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);
        private RSAParameters _privateKey;
        private RSAParameters _publicKey;

        // Khoi tao rsa 
        // public RsaEncryption(){
        //     _privateKey = csp.ExportParameters(true);
        //     _publicKey = csp.ExportParameters(false);
        // }
        // In ra public Key
        public string GetPublicKey(){
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, _privateKey);
            return sw.ToString();
        }
        //Mã hóa key
        public string EncryptionKey(string aesKey){
            csp = new RSACryptoServiceProvider();
            
            csp.ImportParameters(_publicKey);
            var data = Encoding.Unicode.GetBytes(aesKey);
            var cypher = csp.Encrypt(data, false);
            return Convert.ToBase64String(cypher);
        }
       // Giai mã key
        // public string DecryptionKey(string encryptedAesKey){
        //     var dataBytes = encryptedAesKey.ToBase64String();
        //     //encryptedAesKey.FromBase64String(encryptedAesKey);
        //     csp.ImportParameters(_privateKey);
        //     var key = csp.Decrypt(dataBytes, false);
        //     return Encoding.Unicode.GetString(key);
        // }

    }
}