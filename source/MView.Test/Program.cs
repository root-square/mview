using MView.Core.Cryptography;
using System;
using System.Text.RegularExpressions;

namespace MView.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string a = "[1].equips";
            string b = "equips[0]";
            string result;

            string endOfParents = a.Substring(a.LastIndexOf('.') + 1, a.Length - a.LastIndexOf('.') - 1);
            Console.WriteLine("a.b".IndexOf('.', 1 + 1));
            Console.ReadLine();
        }

        private static bool ConnectStrings(string target, string source, out string result)
        {
            char[] sourceChars = source.ToCharArray();

            int offset = target.LastIndexOf(sourceChars[0]);
            int size = target.Length - offset;

            if (offset == -1)
            {
                result = null;
                return false;
            }

            string targetMask = target.Substring(offset, size);

            if (source.StartsWith(targetMask))
            {
                result = target.Remove(offset, size) + source;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}
