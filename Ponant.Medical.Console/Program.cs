namespace Ponant.Medical.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Service.Service service = new Service.Service();
            System.Console.ReadKey();

            //Common.Password.ReinitPassword();
        }
    }
}