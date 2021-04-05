using System.Security.Cryptography;
using System.Text;
using LeakSearchApp.Config;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;

namespace LeakSearchApp.Services
{
    public class HashService : IHashService
    {
        private readonly AppSecrets _appSecrets;
        
        public HashService(IOptions<AppSecrets> appSecrets)
        {
            _appSecrets = appSecrets.Value;
        }

        public byte[] GeneratePrivateHash(string publicHash)
        {
            var saltBytes = Encoding.ASCII.GetBytes(_appSecrets.PrivateSalt);
            return KeyDerivation.Pbkdf2(
                publicHash,
                saltBytes,
                KeyDerivationPrf.HMACSHA512,
                500,
                64);
        }
    }
}