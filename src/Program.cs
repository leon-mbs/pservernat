 
using System.Text;
using System.Text.Json;
 

namespace PrintServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Console.WriteLine("Started...");
            string docPath= AppContext.BaseDirectory ;
 

            builder.Services.AddCors();

            var app = builder.Build();

            var PrinterName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["PrinterName"];


            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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
                    Console.WriteLine("\r\n");

                    Console.WriteLine(e.Message);
             
                    File.WriteAllText(Path.Combine(docPath, "errorlog.txt"), e.Message+"\r\n"+jsonString);

                    return e.Message;
                }
            });

            app.MapGet("/testprintlabel", async (HttpContext httpContext) =>
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
            app.MapGet("/testprintlabel", async (HttpContext httpContext) =>
            {
                try
                {
                    ESC_POS_USB_NET.Printer.Printer printer = new ESC_POS_USB_NET.Printer.Printer(PrinterName, "cp866");
             
                    printer.Append("TEXT 10,10,\"2\",0,1,1,\"Test printer\"");
                    printer.Append("PRINT 1,1");
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
            app.MapGet("/check", async (HttpContext httpContext) =>
            {
                return "Server OK";
            });
           
            app.MapGet("/scale", async (HttpContext httpContext) =>
            {
               return  Results.Json(new { weight = 0, success=true });

          //      if error  return Results.Json(new { error = "error", success = false });

            });

            app.UseCors(builder => builder.AllowAnyOrigin());
            app.Run();
        }
    }
}