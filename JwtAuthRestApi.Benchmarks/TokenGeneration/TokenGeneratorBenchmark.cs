using BenchmarkDotNet.Attributes;

namespace JwtAuthRestApi.Benchmarks.TokenGeneration
{
    /// <summary>
    /// Compare two algorithms of token generation
    /// </summary>
    [MemoryDiagnoser]
    public class TokenGeneratorBenchmark
    {
        private readonly TokenGenerator _tokenGenerator;

        public TokenGeneratorBenchmark()
        {
            _tokenGenerator = new TokenGenerator();
        }

        [Benchmark]
        public string GenerateTokenUsingRng() => _tokenGenerator.GenerateTokenUsingRng();

        [Benchmark]
        public string GenerateTokenUsingXorShift() => _tokenGenerator.GenerateTokenUsingXorShift();
    }
}
