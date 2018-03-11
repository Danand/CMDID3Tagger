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

            if (args.Length == 3)
            {
                if ((args[0].Contains("/") || args[0].Contains("\\")) &&
                    (args[2].Contains("/") || args[2].Contains("\\")))
                {
                    return Args.PathAndStringAndPath;
                }
            }

            throw new ArgumentException();
        }
    }
}
