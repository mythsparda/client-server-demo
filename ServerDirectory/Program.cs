using NetworkLibrary.Directory;

internal class Program
{
    private static void Main(string[] args)
    {
        ServerDirectory server = new ServerDirectory();
        server.Listen();
        (new ManualResetEventSlim(false)).Wait();
    }
}
