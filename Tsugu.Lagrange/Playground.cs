using System.Text.Json;
using System.Text.Json.Serialization;
using Tsugu.Api.Enum;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange;

class A {
    public A(string b) { B = b; }
    public string B { get; set; }
}

public class Playground {
    public static void Api() {
        Console.WriteLine(string.Join("|", Enum.GetNames(typeof(Server)).Select(n => n.ToLower())));
    }
}
