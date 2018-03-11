using CMDID3Tagger.Commands;
using NUnit.Framework;

namespace CMDID3Tagger.Tests.Commands
{
    [TestFixture]
    public class CommandBaseTests
    {
        [TestCase("fromfilename")]
        public void GetCommandTest(string commandString)
        {
            CommandBase command = CommandBase.GetCommand(commandString);
            Assert.AreEqual(commandString, command.CommandString);
        }
    }
}