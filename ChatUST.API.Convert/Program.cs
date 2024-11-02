namespace ChatUST.API.Convert
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });

            builder.Services.AddControllers();
            builder.Services.AddControllers(options =>
            {
                // Add the custom model binder provider at the beginning of the providers list
                options.ModelBinderProviders.Insert(0, new NewtonsoftJsonModelBinderProvider());
            });

            var app = builder.Build();

            app.UseCors("AllowAll");


            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
