using BenchmarkDotNet.Running;
using JwtAuthRestApi.Benchmarks.TokenGeneration;

namespace JwtAuthRestApi.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<TokenGeneratorBenchmark>();
        }
    }
}
