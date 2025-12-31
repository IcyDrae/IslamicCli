namespace IslamicCli
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var handler = new Command.Handler();
            await handler.Execute(args);
        }
    }
}
