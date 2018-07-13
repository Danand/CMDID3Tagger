using System;
using System.Linq;

using CMDID3Tagger.Interfaces;

namespace CMDID3Tagger
{
    public sealed class CommandParser : ICommandParser
    {
        private readonly ICommand[] commands;

        public CommandParser(ICommand[] commands)
        {
            this.commands = commands;
        }

        ICommand ICommandParser.Parse(string commandString)
        {
            var result = commands.FirstOrDefault(command => CheckCommand(command, commandString));

            if (result == null)
                throw new ArgumentException(nameof(commandString));

            return result;
        }

        private bool CheckCommand(ICommand command, string commandString)
        {
            var commandTypeName = command.GetType().Name;
            var lookupString = $"{nameof(ICommand)}{commandString}".Substring(1);

            return string.Equals(commandTypeName, lookupString, StringComparison.OrdinalIgnoreCase);
        }
    }
}