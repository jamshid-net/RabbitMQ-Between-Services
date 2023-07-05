using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace TelegramWebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

       
        builder.Services.AddHostedService<RabbitMqBackground>();
        
        builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(builder?.Configuration?.GetConnectionString("Token")));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
       
        var app = builder.Build();

        

        app.UseHttpsRedirection();


        app.Map("/", () => "Hello");

        app.MapControllers();

        app.Run();
    }
}
