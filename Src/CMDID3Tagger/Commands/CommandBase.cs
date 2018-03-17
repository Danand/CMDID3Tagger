using System;
using System.Collections.Generic;
using System.Linq;

namespace CMDID3Tagger.Commands
{
    /// <summary>
    /// Command base class. Provides getting exact command.
    /// </summary>
    public abstract class CommandBase
    {
        /// <summary>
        /// Command string representation expected from command-line.
        /// </summary>
        public abstract string CommandString { get; }

        /// <summary>
        /// Executes overridden method of suitable command.
        /// </summary>
        /// <param name="args">Command args without command itself.</param>
        public abstract void Execute(string[] args);

        /// <summary>
        /// Gets suitable command by given string.
        /// </summary>
        /// <param name="commandString">Command string representation to be parsed.</param>
        /// <returns>Command object with desired overrides.</returns>
        public static CommandBase GetCommand(string commandString)
        {
            var commands = GetAllCommands();

            var suitableCommand = commands.FirstOrDefault(command => CheckHasGivenCommandString(command, commandString));

            if (suitableCommand == default(CommandBase))
                throw new ArgumentOutOfRangeException(nameof(commandString));

            return suitableCommand;
        }

        private static IEnumerable<CommandBase> GetAllCommands()
        {
            var thisType = typeof(CommandBase);
            var assemblyTypes = thisType.Assembly.GetTypes();
            var commandClasses = assemblyTypes.Where(type => type.IsSubclassOf(thisType));

            return commandClasses.Select(commandClass => (CommandBase)Activator.CreateInstance(commandClass));
        }

        private static bool CheckHasGivenCommandString(CommandBase command, string commandString)
        {
            return string.Equals(command.CommandString, commandString);
        }
    }
}
