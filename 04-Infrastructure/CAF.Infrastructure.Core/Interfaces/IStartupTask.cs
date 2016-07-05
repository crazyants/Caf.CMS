namespace CAF.Infrastructure.Core
{
    public interface IStartupTask 
    {
        void Execute();

        int Order { get; }
    }
}
