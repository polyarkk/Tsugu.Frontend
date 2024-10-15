using Tsugu.Api;
using Tsugu.Api.Enum;

namespace Tsugu.Frontend;

public class Playground {
    public static void Play() {
        Console.WriteLine("""
                          {"op":0,"body":{"self_id":"572264200","platform":"kook","timestamp":1728808916450,"_type":"kook","_data":{"channel_type":"PERSON","type":9,"target_id":"1781061545","author_id":"5
                          72264200","content":"非常好 Satori，爱来自 Satori.Client","extra":{"type":9,"code":"52ac74c842ba97e55b99afaef8f70945","author":{"id":"572264200","username":"polyarkkユ的测试机器人","identify_num":"0371","online":true,"os":"We
                          bsocket","status":0,"avatar":"https://img.kookapp.cn/assets/2024-10/b3QrrFbXVR05g05g.gif?x-oss-process=style/icon","vip_avatar":"https://img.kookapp.cn/assets/2024-10/b3QrrFbXVR05g05g.gif?x-oss-process=image/format,jpg","bann
                          er":"","nickname":"polyarkkユ的测试机器人","roles":[],"is_vip":false,"vip_amp":false,"bot":true,"nameplate":[],"bot_status":0,"tag_info":{"color":"#0096FF","bg_color":"#0096FF33","text":"机器人"},"is_sys":false,"client_id":"B
                          wfXqlZbLQBY28MN","verified":false},"visible_only":null,"mention":[],"mention_all":false,"mention_roles":[],"mention_here":false,"nav_channels":[],"kmarkdown":{"raw_content":"非常好 Satori，爱来自 Satori.Client","mention_part"
                          :[],"mention_role_part":[],"channel_part":[],"spl":[]},"emoji":[],"preview_content":"","last_msg_content":"非常好 Satori，爱来自 Satori.Client","send_msg_device":0},"msg_id":"9d27676c-888a-47ec-99f9-616da6053660","msg_timesta
                          mp":1728808913863,"nonce":"","from_type":1},"type":"message-created","channel":{"type":1,"id":"52ac74c842ba97e55b99afaef8f70945"},"member":{"nick":"polyarkkユ的测试机器人"},"user":{"id":"572264200","name":"polyarkkユ的测试机 
                          器人","user_id":"572264200","avatar":"https://img.kookapp.cn/assets/2024-10/b3QrrFbXVR05g05g.gif?x-oss-process=style/icon","username":"polyarkkユ的测试机器人","discriminator":"0371"},"message":{"content":"非常好 Satori，爱来 
                          自 Satori.Client","elements":[{"type":"text","attrs":{"content":"非常好 Satori，爱来自 Satori.Client"},"children":[]}],"user":{"id":"572264200","name":"polyarkkユ的测试机器人","user_id":"572264200","avatar":"https://img.kooka
                          pp.cn/assets/2024-10/b3QrrFbXVR05g05g.gif?x-oss-process=style/icon","username":"polyarkkユ的测试机器人","discriminator":"0371"},"member":{"user":{"id":"572264200","name":"polyarkkユ的测试机器人","user_id":"572264200","avatar"
                          :"https://img.kookapp.cn/assets/2024-10/b3QrrFbXVR05g05g.gif?x-oss-process=style/icon","username":"polyarkkユ的测试机器人","discriminator":"0371"},"nick":"polyarkkユ的测试机器人"},"message_id":"9d27676c-888a-47ec-99f9-616da6053660","id":"9d27676c-888a-47ec-99f9-616da6053660","timestamp":1728808913863},"id":62}}
                          """.Replace("\n", "")
        );
    }
}
