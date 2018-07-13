namespace CMDID3Tagger.Interfaces
{
    public interface ICommandParser
    {
        ICommand Parse(string commandString);
    }
}