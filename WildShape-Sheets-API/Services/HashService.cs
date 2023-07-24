using System.Security.Cryptography;
using System.Text;

namespace WildShape_Sheets_API.Services {
    public class HashService {

        private readonly DataBaseService _dataBaseService;
        private readonly IConfiguration _configuration;
        private readonly int _keySize;
        private readonly int _iterations;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public HashService(DataBaseService dataBaseService, IConfiguration configuration)
        {
            _configuration = configuration;
            _dataBaseService = dataBaseService;

            _keySize = int.Parse(_configuration.GetSection("Hashbrowns:KeySize").Value!);
            _iterations = int.Parse(_configuration.GetSection("Hashbrowns:Iterations").Value!);
        }

        internal string HashPassword(string password, out byte[] salt) {
            salt = RandomNumberGenerator.GetBytes(_keySize);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                _iterations,
                hashAlgorithm,
                _keySize);

            return Convert.ToHexString(hash);
        }

        public bool VerifyPasswordHash(string password, string hash, byte[] salt) {

            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, hashAlgorithm, _keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }
        public string GetSHA256Hash(string input)
        {
            //ComputeSHA256Hash
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Convert the byte array to a hexadecimal string.
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
