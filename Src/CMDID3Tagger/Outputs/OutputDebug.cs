using System.Diagnostics;

using CMDID3Tagger.Interfaces;

namespace CMDID3Tagger.Outputs
{
    public sealed class OutputDebug : IOutput
    {
        void IOutput.Write(string message)
        {
            Debug.WriteLine(message);
        }
    }
}