using System;
using System.Collections;
using System.Text;

namespace XorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var packet1 = Encoding.ASCII.GetBytes("To jest test xora");
            var packet2 = Encoding.ASCII.GetBytes("aa bb cc dd ee ff");
            var packet3 = Encoding.ASCII.GetBytes("gg hh ii jj kk ll");
            var xor = CalculateXor(packet1, packet2, packet3);

            Console.WriteLine($"XOR: {Encoding.ASCII.GetString(xor)}");

            Console.WriteLine($"Retrieve1: {Encoding.ASCII.GetString(CalculateXor(xor, packet2, packet3))}");
            Console.WriteLine($"Retrieve2: {Encoding.ASCII.GetString(CalculateXor(packet1, xor, packet3))}");
            Console.WriteLine($"Retrieve3: {Encoding.ASCII.GetString(CalculateXor(packet1, packet2, xor))}");
            Console.ReadLine();
        }

        private static byte[] CalculateXor(params byte[][] packets)
        {
            BitArray bits = new BitArray(packets[0]);
            for (var i = 1; i < packets.Length; i++)
            {
                bits = bits.Xor(new BitArray(packets[i]));
            }

            var res = new byte[packets[0].Length];
            bits.CopyTo(res, 0);
            return res;
        }
    }
}
