using Microsoft.Extensions.Configuration;

namespace Lagrange.Tsugu;

class A {
    public string GetType2() { return GetType().Name; }
}

class B : A { }

public class Playground {
    public static void Api() {
        var conf = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json")
            .Build();
        
        Console.WriteLine(conf.GetSection("Tsugu").Get<AppSettings>()!.Friends[0]);
    }
}
