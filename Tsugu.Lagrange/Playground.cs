using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tsugu.Lagrange;

class A {
    public A(string b) { B = b; }
    public string B { get; set; }
}

public class Playground {
    public static void Api() {
        Console.WriteLine("""
                          {"b": "haha"}
                          """.DeserializeJson<A>().B
        );
    }
}
