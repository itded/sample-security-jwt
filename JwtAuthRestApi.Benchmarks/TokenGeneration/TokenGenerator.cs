using System;
using System.Security.Cryptography;

namespace JwtAuthRestApi.Benchmarks.TokenGeneration
{
    public class TokenGenerator
    {
        public string GenerateTokenUsingRng()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var token = Convert.ToBase64String(randomNumber);
            return token;
        }

        public string GenerateTokenUsingXorShift()
        {
            var randomNumber = new byte[32];
            new XorShiftRandom().NextBytes(randomNumber);
            var token = Convert.ToBase64String(randomNumber);
            return token;
        }
    }
}
