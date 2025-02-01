namespace VBAN_Studio.Common.Command
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        void Execute(string[] args);
    }

    public abstract class Command : ICommand
    {
        public abstract string Name { get; }

        public abstract string Description { get;}

        public abstract void Execute(string[] tokens);
    }
}
