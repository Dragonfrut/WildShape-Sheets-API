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
    }
}
