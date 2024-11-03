 
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


            //проверка  что  запущке
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

            //тест  с  браузера
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
            //тест  с  браузера для  режима label
            app.MapGet("/testprintlabel", async (HttpContext httpContext) =>
            {
                try
                {
                    ESC_POS_USB_NET.Printer.Printer printer = new ESC_POS_USB_NET.Printer.Printer(PrinterName, "cp866");

                    printer.Append("CLS");
                    printer.Append("DIRECTION 1");
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

            //вкс  с  вксов
            app.MapGet("/scale", async (HttpContext httpContext) =>
            {
               
                var req = httpContext.Request;
                try
                {
                     

                    double weight = 0;   //gtodo  получаем вес с весовd

                    var resp = new RespData {
                        success = true,
                        weight= weight
                    };
                    return JsonSerializer.Serialize(resp);



                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);

                    var resp = new RespData
                    {
                        success = false,
                        error= e.Message
                    };
                    return JsonSerializer.Serialize(resp);
                }
            });

             
            app.MapPost("/bank", async (HttpContext httpContext) =>
            {
                var reqdata = new ReqBankData();
                var req = httpContext.Request;
                try
                {
                    using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true))
                    {
                        var jsonString = await reader.ReadToEndAsync();
                        reqdata = JsonSerializer.Deserialize<ReqBankData>(jsonString);
                    }


                    double weight = 55;   //gtodo  получаем вес с весовd

                    var resp = new RespData
                    {
                        success = true,
                        weight = weight
                    };
                    return JsonSerializer.Serialize(resp);



                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);

                    var resp = new RespData
                    {
                        success = false,
                        error = e.Message
                    };
                    return JsonSerializer.Serialize(resp);
                }
            });
           
            app.UseCors(builder => builder.AllowAnyOrigin());
            app.Run();
        }
    }

    public  class ReqBankData
    {
        public string mfo { get; set; }   

    }
    public class RespData
    {
        public double weight { get; set; }   //вес  с электронных весов
        public bool success { get; set; }  //true если  нет ощибок
        public string? error { get; set; }  //сообщение об ошиьке


    }

}