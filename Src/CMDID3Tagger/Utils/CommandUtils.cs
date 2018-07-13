using System;
using System.Linq;

using CMDID3Tagger.Interfaces;

namespace CMDID3Tagger.Utils
{
    public static class CommandUtils
    {
        public static Type[] GetAllCommandTypes()
        {
            var commandInterface = typeof(ICommand);

            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(assembly => assembly.GetTypes())
                            .Where(type => commandInterface.IsAssignableFrom(type) && !type.IsInterface).ToArray();
        }
    }
}