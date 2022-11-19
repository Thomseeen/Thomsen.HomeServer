using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;
using System.Reflection;

using Thomsen.HomeServer.Core;
using Thomsen.HomeServer.Core.InfluxDb;
using Thomsen.HomeServer.Widget.ViewModels;

namespace Thomsen.HomeServer.Widget;

public class Program {
    [STAThread]
    public static void Main(string[] args) {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>()
            .Build();

        ServiceCollection services = new();
        services.AddSingleton(configuration);

        services.Configure<WidgetOptions>(configuration.GetSection("Widget"));

        services.AddInfluxDbClient(configuration.GetSection("InfluxDb").Get<InfluxDbOptions>());
        services.AddTransient<IInfluxDbAdapter, InfluxDbAdapter>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<TaskbarIconViewModel>();

        services.AddSingleton<HomeServerWidget>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        HomeServerWidget application = serviceProvider.GetRequiredService<HomeServerWidget>();

        application.InitializeComponent();
        application.Run();
    }
}
