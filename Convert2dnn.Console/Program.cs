using LINQPad_Interactive.Convert.Lib;
using System;

namespace Convert2dnn.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0) {
                FileConvert.ConvertToInteractiveNotebook(args[0]);
            }

            else
            {
                System.Console.WriteLine("usage: ..exe \"inputPath.linq\"");
            }
        }
    }
}
