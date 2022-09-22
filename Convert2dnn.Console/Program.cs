using LINQPad_Interactive.Convert.Lib;
using System;

namespace Convert2dnn_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0) {
                var status = FileConvert.ConvertToInteractiveNotebook(args[0]);
                Console.WriteLine(status);
            }

            else
            {
                Console.WriteLine("usage: ..exe \"inputPath.linq\"");
            }
        }
    }
}
