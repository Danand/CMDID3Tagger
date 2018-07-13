using System;

using CMDID3Tagger.Interfaces;

namespace CMDID3Tagger.Outputs
{
    public sealed class OutputConsole : IOutput
    {
        void IOutput.Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}