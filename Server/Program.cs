namespace Server
{
    internal class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.WriteLine("Server is running...");
                System.Threading.Thread.Sleep(5000);
            }
        }
    }
}
