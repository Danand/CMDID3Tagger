using System;

namespace CMDID3Tagger
{
    internal static class ArgsParser
    {
        public static Args Parse(string[] args)
        {
            if (args.Length == 2)
            {
                if (args[0].Contains("/") || args[0].Contains("\\"))
                    return Args.PathAndString;
            }

            throw new ArgumentException();
        }
    }
}
