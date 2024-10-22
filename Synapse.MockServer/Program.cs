namespace Synapse.MockServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.


            app.MapControllers();

            app.Run();
        }
    }
}
