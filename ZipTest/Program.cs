using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using ZstdNet;

namespace ZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var trainingDict = Train(PrepareTrainingData(500));
            var message = $"Id=12345678;SystemName='Abcdef'; Index='45'; TimeStamp={DateTime.UtcNow}; State1=Normal; State2=22234564646456;";
            var packet = Encoding.ASCII.GetBytes(message);

            var normalCompression = Compress(packet, new CompressionOptions(15));
            var dictionaryCompression = Compress(packet, new CompressionOptions(trainingDict, 15));

            Console.WriteLine(message);
            Console.WriteLine($"Data length: {packet.Length} {packet}");
            Console.WriteLine($"Normal compression: {normalCompression.Length}");
            Console.WriteLine($"Dictionary compression: {dictionaryCompression.Length}");

            Console.ReadLine();
        }

        private static byte[] Compress(byte[] input, CompressionOptions compressionOptions)
        {
            var dataStream = new MemoryStream(input);

            var resultStream = new MemoryStream();
            using (var compressionStream = new CompressionStream(resultStream, compressionOptions))
                dataStream.CopyTo(compressionStream);
            return resultStream.ToArray();
        }


        private static IEnumerable<string> PrepareTrainingData(int count)
        {
            var states1 = new[] { "Normal", "Warning", "Error" };
            var states2 = new[] { "123035345353535", "543134545664545", "22234564646456", "567436456456" };
            var timestamp = DateTime.UtcNow;
            for (var i = 0; i < count; i++)
            {
                timestamp = timestamp.AddSeconds(1);
                yield return $"Id=12345678;SystemName='Abcdef'; Index='{i}'; TimeStamp={timestamp}; State1={states1[i%3]}; State2={states2[i % 4]};";
            }
        }

        private static byte[] Train(IEnumerable<string> data)
        {
            var trainingData = data.Select(d => Encoding.ASCII.GetBytes(d));
            return DictBuilder.TrainFromBuffer(trainingData);
        }
    }
}
