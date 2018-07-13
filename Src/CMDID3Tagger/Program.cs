using System;
using System.Linq;

using CMDID3Tagger.Interfaces;
using CMDID3Tagger.Outputs;
using CMDID3Tagger.Utils;

using MicroResolver;

namespace CMDID3Tagger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var resolver = ObjectResolver.Create();
            var commandTypes = CommandUtils.GetAllCommandTypes();

            resolver.Register<ITagPropertyEditor, TagPropertyEditor>(Lifestyle.Singleton);
            resolver.Register<IOutput, OutputConsole>(Lifestyle.Singleton);
            resolver.Register<ICommandParser, CommandParser>(Lifestyle.Singleton);

            resolver.RegisterCollection<ICommand>(Lifestyle.Singleton, commandTypes);

            resolver.Compile();

            var commandParser = resolver.Resolve<ICommandParser>();

            var commandString = args[0];
            var commandArgs = args.Skip(1).ToArray();

            var command = commandParser.Parse(commandString);
            command.Execute(commandArgs);

            Console.WriteLine("Done. Press any key to continue...");
            Console.ReadLine();
        }
    }
}
