using System;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Base64Converter
{
    [MemoryDiagnoser]
    public class PerfClass {
        private readonly int iterCount = 10000;
        private readonly string toConvert = "no so long string to convert";
        private readonly Converter customConverter = new Converter();

        [Benchmark]
        public void CustomConverterTest() {
            for (int i = 0; i < iterCount; i++)
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(toConvert);
                var result = customConverter.Encode(plainTextBytes);
            }
        }

        [Benchmark]
        public void EmbededConverterTest() {
            for (int j = 0; j < iterCount; j++)
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(toConvert);
                var result = Convert.ToBase64String(plainTextBytes);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<PerfClass>();

            // var converter = new Converter();
            // var str = string.Empty;

            // do {
            //     Console.WriteLine("Go:");
            //     str = Console.ReadLine();
            //     var plainTextBytes = Encoding.UTF8.GetBytes(str);
            //     Console.WriteLine(converter.Encode(plainTextBytes));
            // } while (str != "q");
        }
    }
}
