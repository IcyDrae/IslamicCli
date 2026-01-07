using IslamicCli.Command;

namespace IslamicCli
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var handler = new Handler();
            await handler.Execute(args);
        }
    }
}
