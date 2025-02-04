namespace VBAN_Studio.Common.Command
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        void Execute(VbanStudioEnvironment _environment, string[] args);
    }

    public abstract class Command : ICommand
    {
        public abstract string Name { get; }

        public abstract string Description { get;}

        public abstract void Execute(VbanStudioEnvironment _environment, string[] tokens);
    }
}
