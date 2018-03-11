using System.Linq;
using CMDID3Tagger.Commands;

namespace CMDID3Tagger
{
    /// <summary>
    /// Tag command wrapper.
    /// </summary>
    public class Tagger
    {
        /// <summary>
        /// Chooses and runs suitable command from given arguments.
        /// </summary>
        /// <param name="args">Command iteself and it's arguments.</param>
        public static void Start(string[] args)
        {
            // Commands are case-insensitive:
            string commandString = args[0].ToLower(); 

            CommandBase command = CommandBase.GetCommand(commandString);

            // Skipping command itself:
            args = args.Skip(1).ToArray(); 

            command.Execute(args);
        }
    }
}
