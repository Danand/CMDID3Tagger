using System;

namespace CMDID3Tagger
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Tagger.Start(args);
            Console.WriteLine("Done. Press any key to continue...");
            Console.ReadLine();
        }
    }
}
