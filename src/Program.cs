 
using System.Text;
using System.Text.Json;
 

namespace PServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Console.WriteLine("Statrted...");
            builder.Services.AddCors();

            var app = builder.Build();

            var PrinterName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["PrinterName"];


            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            app.MapGet("/check", async (HttpContext httpContext) =>
            {
                return "POS server: OK";
            });

            app.MapPost("/print", async (HttpContext httpContext) =>
            {
                ESC_POS_USB_NET.Printer.Printer printer = new ESC_POS_USB_NET.Printer.Printer(PrinterName, "cp866");


                var jsonString = "";
                var req = httpContext.Request;
                try { 
                    using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true))
                    {
                        jsonString = await reader.ReadToEndAsync();
                    }

                    var list = JsonSerializer.Deserialize<List<byte>>(jsonString);

                    printer.Append(list.ToArray());
                    printer.PrintDocument();
                    return "";



                }catch(Exception e)
                {

                    Console.WriteLine(e.Message);

                    return e.Message;
                }
            });

            app.MapGet("/testprint", async (HttpContext httpContext) =>
            {
                try
                {
                    ESC_POS_USB_NET.Printer.Printer printer = new ESC_POS_USB_NET.Printer.Printer(PrinterName, "cp866");

                    printer.Append("Test printer");
                    printer.Append("Тест принтера");
                    printer.Append(new byte[] { 0x0a });
                    printer.PrintDocument();
                    printer.Clear();
                    Console.WriteLine("Тeст принтера");
                    return "Тeст принтера";
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);

                    return e.Message;
                }
            });


            app.UseCors(builder => builder.AllowAnyOrigin());
            app.Run();
        }
    }
}