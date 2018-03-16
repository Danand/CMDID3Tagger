using CMDID3Tagger.Commands;
using NUnit.Framework;

namespace CMDID3Tagger.Tests.Commands
{
    [TestFixture]
    public class CommandBaseTests
    {
        [TestCase("fromfilename")]
        [TestCase("fromtags")]
        public void GetCommand(string commandString)
        {
            CommandBase command = CommandBase.GetCommand(commandString);
            Assert.AreEqual(command.CommandString, commandString);
        }
    }
}