namespace WebAppPublisher.Configuration
{
    public static class StartupConfiguration
    {
        public static WebApplicationBuilder UseStartup<TStartup>(this WebApplicationBuilder webApplicationBuilder) where TStartup : WebAppPublisher.Configuration.IStartup
        {
            var startup = (TStartup)Activator.CreateInstance(typeof(TStartup), webApplicationBuilder.Configuration);

            ArgumentNullException.ThrowIfNull("Erro ao inicializar o Startup");

            startup.ConfigureServices(webApplicationBuilder.Services);

            var app = webApplicationBuilder.Build();

            startup.Configure(app, webApplicationBuilder.Environment);

            app.Run();

            return webApplicationBuilder;
        }
    }
}