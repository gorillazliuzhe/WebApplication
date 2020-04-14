using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using WebApplication.Filters;

namespace WebApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await CreateHostBuilder(args).Build().RunWithTasksAsync();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
#if !DEBUG
                    webBuilder.UseUrls("http://0.0.0.0:5006/"); // ����ʹ��"http://*:5000"����ô������Ѷ�Ʋ�������          
#endif
                });
    }
}
