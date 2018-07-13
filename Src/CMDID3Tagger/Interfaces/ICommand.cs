namespace CMDID3Tagger.Interfaces
{
    public interface ICommand
    {
        void Execute(params string[] args);
    }
}
