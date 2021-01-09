using MView.Core.Cryptography;
using System;

namespace MView.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            CryptographyProvider.RestoreOggHeader(@"E:\MVTest\Battle1.rpgmvo", @"E:\MVTest\Battle1-My.ogg");
            Console.WriteLine("Completed.");
            Console.ReadLine();
        }
    }
}
