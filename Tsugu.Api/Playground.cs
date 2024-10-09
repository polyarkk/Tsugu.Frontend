using Tsugu.Api.Entity;
using Tsugu.Api.Misc;

namespace Tsugu.Api;

public class Playground {
    public static void A() {
        var tsugu = new TsuguClient("http://127.0.0.1:3000");
        
        var task = tsugu.Station.QueryAllRoom();

        task.Wait();

        var rooms = task.Result;

        var task2 = tsugu.Query.RoomList(rooms);

        task2.Wait();

        var result = task2.Result;

        byte[] b = Convert.FromBase64String(result);
            
        File.WriteAllBytes(@"C:\Users\17811\Desktop\" + Random.Shared.NextInt64(), b);
    }

    public static void B() {
        var tsugu = new TsuguClient("http://127.0.0.1:3000");

        Console.WriteLine("""
                          {
                              "_id": "red:1781176460",
                              "userId": "1781176460",
                              "platform": "red",
                              "mainServer": 3,
                              "displayedServerList": [
                                  3,
                                  0
                              ],
                              "shareRoomNumber": true,
                              "userPlayerIndex": 0,
                              "userPlayerList": []
                          }
                          """.DeserializeJson<TsuguUser>().Id
        );
    }
}
