﻿using System.Text.RegularExpressions;
using Tsugu.Api.Util;

namespace Tsugu.Api.Misc;

/// <summary>
/// 模糊搜索过滤器的 C# 实现
/// </summary>
public static partial class FuzzySearch {
    /// <summary>
    /// <inheritdoc cref="FuzzySearch"/>
    /// </summary>
    /// <param name="keywords">将要查询的查询参数。</param>
    /// <returns>匹配结果</returns>
    public static Dictionary<string, List<object>> Parse(string[] keywords) {
        Dictionary<string, List<object>> matches = new();

        foreach (string k in keywords) {
            bool matched = false;
            string keyword = k.ToLower();

            if (int.TryParse(keyword, out int i)) {
                AddToMatches(matches, "_number", i);
                continue;
            }

            keyword = keyword.Replace("&gt;", ">");
            keyword = keyword.Replace("&lt;", "<");
            keyword = keyword.Replace("＞", ">");
            keyword = keyword.Replace("＜", "<");

            int? lvNumber = ExtractLvNumber(keyword);

            if (lvNumber.HasValue) {
                AddToMatches(matches, "songLevels", lvNumber.Value);
                continue;
            }

            if (IsValidRelationStr(keyword)) {
                AddToMatches(matches, "_relationStr", keyword);
                continue;
            }

            foreach (string type in Patterns.Keys) {
                Dictionary<string, string[]> typeConfig = Patterns[type];

                foreach (string key in
                    from key in typeConfig.Keys
                    let values = typeConfig[key]
                    where values.Any<string>(value => value == keyword)
                    select key
                ) {
                    AddToMatches(matches, type, StringToIntOrNull(key) ?? (object)key);
                    matched = true;
                }
            }

            if (!matched) {
                AddToMatches(matches, "_all", k);
            }
        }

        return matches;
    }

    [GeneratedRegex(@"^lv(\d+)$")]
    private static partial Regex LvRegex();

    private static int? ExtractLvNumber(string str) {
        Regex regex = LvRegex();
        Match match = regex.Match(str);

        if (match.Success && match.Groups[1].Success) {
            return int.Parse(match.Groups[1].Value);
        }

        return null;
    }

    [GeneratedRegex(@"^<\d+$")]
    private static partial Regex LessThanRegex();

    [GeneratedRegex(@"^>\d+$")]
    private static partial Regex GreaterThanRegex();

    [GeneratedRegex(@"^\d+-\d+$")]
    private static partial Regex RangeRegex();

    private static bool IsValidRelationStr(string relationStr) {
        Regex lessThanRegex = LessThanRegex();
        Regex greaterThanRegex = GreaterThanRegex();
        Regex rangeRegex = RangeRegex();

        return lessThanRegex.IsMatch(relationStr)
            || greaterThanRegex.IsMatch(relationStr)
            || rangeRegex.IsMatch(relationStr);
    }

    private static void AddToMatches(Dictionary<string, List<object>> matches, string key, object value2) {
        if (matches.TryGetValue(key, out List<object>? value)) {
            value.Add(value2);
        } else {
            matches[key] = [value2];
        }
    }

    private static int? StringToIntOrNull(string s) {
        if (int.TryParse(s, out int i)) {
            return i;
        }

        return null;
    }

    private readonly static Dictionary<string, Dictionary<string, string[]>> Patterns =
        """{"characterId":{"1":["戸山香澄","戸山香澄","kasumitoyama","kasumitoyama","戶山香澄","戶山香澄","户山香澄","户山香澄","香澄","kasumi","香澄","香澄","戸山","toyama","戶山","户山","ksm","cdd","dd","猫猫头","猫耳","香香","小香","澄澄","小香澄","香澄酱","gt.vo.","vo.gt.","gt.","vo.","吉他","主唱","gtvo","vogt","gt","vo","非凡之星"],"2":["花園たえ","花園たえ","taehanazono","taehanazono","花園多惠","花園多惠","花园多惠","花园多惠","たえ","tae","多惠","多惠","花園","hanazono","花園","花园","otae","惠惠","多英","小花","小惠","gt.","吉他","gt","追兔花园"],"3":["牛込りみ","牛込りみ","rimiushigome","rimiushigome","牛込里美","牛込里美","牛込里美","牛込里美","りみ","rimi","里美","里美","牛込","ushigome","牛込","牛込","李美丽","螺","丽丽","阿丽","rimili","rmr","ba.","贝斯","ba","告一段「螺」","告一段螺"],"4":["山吹沙綾","山吹沙綾","sayayamabuki","sayayamabuki","山吹沙綾","山吹沙綾","山吹沙绫","山吹沙绫","沙綾","saya","沙綾","沙绫","山吹","yamabuki","山吹","山吹","saaya","dr.","鼓手","dr","发酵少女"],"5":["市ヶ谷有咲","市ヶ谷有咲","arisaichigaya","arisaichigaya","市谷有咲","市谷有咲","市谷有咲","市谷有咲","有咲","arisa","有咲","有咲","市ヶ谷","ichigaya","市谷","市谷","arisa","ars","秋妈妈","小咲","key.","键盘","key","甜辣个性"],"6":["美竹蘭","美竹蘭","ranmitake","ranmitake","美竹蘭","美竹蘭","美竹兰","美竹兰","蘭","ran","蘭","兰","美竹","mitake","美竹","美竹","lan","兰兰","小兰","兰酱","gt.vo.","vo.gt.","gt.","vo.","吉他","主唱","gtvo","vogt","gt","vo","叛逆的红挑染"],"7":["青葉モカ","青葉モカ","mocaaoba","mocaaoba","青葉摩卡","青葉摩卡","青叶摩卡","青叶摩卡","モカ","moca","摩卡","摩卡","青葉","aoba","青葉","青叶","毛力","moka","mocachan","摩卡酱","小摩卡","卡卡","gt.","吉他","gt","GOMYWAY"],"8":["上原ひまり","上原ひまり","himariuehara","himariuehara","上原緋瑪麗","上原緋瑪麗","上原绯玛丽","上原绯玛丽","ひまり","himari","緋瑪麗","绯玛丽","上原","uehara","上原","上原","肥玛丽","肥绯","hmr","aao","ba.","贝斯","ba","一呼零应"],"9":["宇田川巴","宇田川巴","tomoeudagawa","tomoeudagawa","宇田川巴","宇田川巴","宇田川巴","宇田川巴","巴","tomoe","巴","巴","宇田川","udagawa","宇田川","宇田川","tme","巴姐","巴哥","soiya","dr.","鼓手","dr","豚骨酱油大姐头"],"10":["羽沢つぐみ","羽沢つぐみ","tsugumihazawa","tsugumihazawa","羽澤鶇","羽澤鶇","羽泽鸫","羽泽鸫","つぐみ","tsugumi","鶇","鸫","羽沢","hazawa","羽澤","羽泽","茨菇","茨菇","刺骨","tsugu","tsg","342g","鸫鸫","key.","键盘","key","伟大的平凡"],"11":["弦巻こころ","弦巻こころ","kokorotsurumaki","kokorotsurumaki","弦卷心","弦卷心","弦卷心","弦卷心","こころ","kokoro","心","心","弦巻","tsurumaki","弦卷","弦卷","kkr","心心","心仔","小心心","富婆心","vo.","主唱","vo","笑容攻势如潮"],"12":["瀬田薫","瀬田薫","kaoruseta","kaoruseta","瀨田薰","瀨田薰","濑田薰","濑田薰","薫","kaoru","薰","薰","瀬田","seta","瀨田","濑田","儚い","梦幻","夢幻","薰哥哥","薰哥","薰酱","哈卡奶","hakanai","烤炉","gt.","吉他","gt","荒诞无稽的独角戏"],"13":["北沢はぐみ","北沢はぐみ","hagumikitazawa","hagumikitazawa","北澤育美","北澤育美","北泽育美","北泽育美","はぐみ","hagumi","育美","育美","北沢","kitazawa","北澤","北泽","hgm","育育","ba.","贝斯","ba","北泽象征元气"],"14":["松原花音","松原花音","kanonmatsubara","kanonmatsubara","松原花音","松原花音","松原花音","松原花音","花音","kanon","花音","花音","松原","matsubara","松原","松原","水母","卡农","小花音","dr.","鼓手","dr","迷宫中的水母"],"15":["奥沢美咲","奥沢美咲","misakiokusawa","misakiokusawa","奧澤美咲","奧澤美咲","奥泽美咲","奥泽美咲","美咲","misaki","美咲","美咲","奥沢","okusawa","奧澤","奥泽","米歇尔","米歇爾","熊","熊","michele","msk","米谢噜","dj","有常识的熊"],"16":["丸山彩","丸山彩","ayamaruyama","ayamaruyama","丸山彩","丸山彩","丸山彩","丸山彩","彩","aya","彩","彩彩","丸山","maruyama","丸山","丸山","小彩","彩酱","彩姐","阿彩","vo.","主唱","vo","疯狂的自我搜索者"],"17":["氷川日菜","氷川日菜","hinahikawa","hinahikawa","冰川日菜","冰川日菜","冰川日菜","冰川日菜","日菜","hina","日菜","日菜","氷川","hikawa","冰川","冰川","噜","run","17","gt.","吉他","gt","隔壁的小天才"],"18":["白鷺千聖","白鷺千聖","chisatoshirasagi","chisatoshirasagi","白鷺千聖","白鷺千聖","白鹭千圣","白鹭千圣","千聖","chisato","千聖","千圣","白鷺","shirasagi","白鷺","白鹭","cst","小千","千酱","小千圣","ba.","贝斯","ba","微笑的铁假面"],"19":["大和麻弥","大和麻弥","mayayamato","mayayamato","大和麻彌","大和麻彌","大和麻弥","大和麻弥","麻弥","maya","麻彌","麻弥","大和","yamato","大和","大和","大和妈咪","妈咪","妈呀","玛雅","dr.","鼓手","dr","狂暴的器材宅"],"20":["若宮イヴ","若宮イヴ","evewakamiya","evewakamiya","若宮伊芙","若宮伊芙","若宫伊芙","若宫伊芙","イヴ","eve","伊芙","伊芙","若宮","wakamiya","若宮","若宫","if","武士道","阿芙","阿福","伊芙酱","小伊芙","小芙","bushido","key.","键盘","key","来自北欧的武士"],"21":["湊友希那","湊友希那","yukinaminato","yukinaminato","湊友希那","湊友希那","凑友希那","凑友希那","友希那","yukina","友希那","友希那","湊","minato","湊","凑","有希那","ykn","企鹅","憋笑","i83","凑女人","猫奴","vo.","主唱","vo","狂乱绽放的紫炎蔷薇"],"22":["氷川紗夜","氷川紗夜","sayohikawa","sayohikawa","冰川紗夜","冰川紗夜","冰川纱夜","冰川纱夜","紗夜","sayo","紗夜","纱夜","氷川","hikawa","冰川","冰川","342g","34","gt.","吉他","gt","忧伤节拍器"],"23":["今井リサ","今井リサ","lisaimai","lisaimai","今井莉莎","今井莉莎","今井莉莎","今井莉莎","リサ","lisa","莉莎","莉莎","今井","imai","今井","今井","risa","Lisa姐","Lisa内","锂砂镍","ba.","贝斯","ba","慈爱女神"],"24":["宇田川あこ","宇田川あこ","akoudagawa","akoudagawa","宇田川亞子","宇田川亞子","宇田川亚子","宇田川亚子","あこ","ako","亞子","亚子","宇田川","udagawa","宇田川","宇田川","阿仔","小亚子","dr.","鼓手","dr","引起黑暗波动略黑的坠天使"],"25":["白金燐子","白金燐子","rinkoshirokane","rinkoshirokane","白金燐子","白金燐子","白金燐子","白金燐子","燐子","rinko","燐子","燐子","白金","shirokane","白金","白金","rinrin","燐","燐","燐燐","燐仔","燐姐","小燐","燐可","……","提词姬","00","提词器","key.","键盘","key","稳如磐石的大触"],"26":["倉田ましろ","倉田ましろ","mashirokurata","mashirokurata","倉田真白","倉田真白","ましろ","mashiro","真白","倉田","kurata","倉田","仓田真白","仓田真白","msr","vo.","主唱","vo","向后全速前进"],"27":["桐ヶ谷透子","桐ヶ谷透子","tokokirigaya","tokokirigaya","桐谷透子","桐谷透子","透子","toko","透子","桐ヶ谷","kirigaya","桐谷","tk","gt.","吉他","gt","天上天下唯我独尊"],"28":["広町七深","広町七深","nanamihiromachi","nanamihiromachi","廣町七深","廣町七深","七深","nanami","七深","広町","hiromachi","廣町","nnm","ba.","贝斯","ba","我说了什么奇怪的话吗"],"29":["二葉つくし","二葉つくし","tsukushifutaba","tsukushifutaba","二葉筑紫","二葉筑紫","つくし","tsukushi","筑紫","二葉","futaba","二葉","土笔","土筆","tks","二叶筑紫","二叶","筑比","dr.","鼓手","dr","长大的Girl"],"30":["八潮瑠唯","八潮瑠唯","ruiyashio","ruiyashio","八潮瑠唯","八潮瑠唯","瑠唯","rui","瑠唯","八潮","yashio","八潮","yso","vn.","vn","小提琴","正论暴击机"],"31":["和奏レイ","和奏レイ","reiwakana","reiwakana","和奏瑞依","和奏瑞依","レイ","rei","瑞依","和奏","wakana","和奏","レイヤ","layer","layer","大姐头","ba.vo.","vo.ba.","ba.","贝斯","vo.","主唱","voba","bavo","vo","ba","慕鳴歌瑠","容易被人叫成姐"],"32":["朝日六花","朝日六花","rokkaasahi","rokkaasahi","朝日六花","朝日六花","六花","rokka","六花","朝日","asahi","朝日","ロック","lock","lock","gt.","吉他","gt","義侘唖","吉他狂战士"],"33":["佐藤ますき","佐藤ますき","masukisato","masukisato","佐藤益木","佐藤益木","ますき","masuki","益木","佐藤","sato","佐藤","マスキング","masking","masking","狂犬","狂犬","king","dr.","鼓手","dr","怒罹夢","无赖鼓手人情派"],"34":["鳰原令王那","鳰原令王那","reonanyubara","reonanyubara","鳰原令王那","鳰原令王那","令王那","reona","令王那","鳰原","nyubara","鳰原","パレオ","pareo","pareo","暗黑丸山彩","key.","键盘","key","嬉鋳牡音","忠犬PAREO公","忠犬PARE公"],"35":["珠手ちゆ","珠手ちゆ","chiyutamade","chiyutamade","珠手知由","珠手知由","ちゆ","chiyu","知由","珠手","tamade","珠手","チュチュ","chu2","chu²","chuchu","chu平方","猫耳","猫耳","楚萍芳","chu","dj","円盤騎手","小矮子革命儿"],"36":["高松燈","高松燈","tomori","Tomori","燈","高松灯","高松灯","高松","takamatsutomori","takamatsutomori","灯","羽丘诗人","tmr","灯灯","tomorin","Tomorin","灯宝","羽丘的不可思议酱","羽丘の不思議ちゃん","小动物","羽丘企鹅","主唱太拼命了","水族馆年卡","vo.","主唱","vo"],"37":["千早愛音","千早愛音","chihayaanon","chihayaanon","Anon","anon","anon酱","ano酱","愛音","爱音","千早","千早爱音","anochan","Anochan","羽丘渣女","AnonTokyo","AnonTokyo","anontokyo","英国女同","吉他","水族馆年卡","gt.","gt","出路","失败"],"38":["要楽奈","要楽奈","要乐奈","要","乐奈","要乐奈","kanamerāna","kanamerāna","kanamerana","kanamerana","Rāna","rāna","rana","Aana","noraneko","野猫","猫","抹茶","抹茶芭菲","楽奈","吉他","gt.","gt","希望","归宿"],"39":["長崎そよ","長崎そよ","长崎素世","长崎","长崎素世","长崎爽世","长崎爽世","Soyo","soyo","そよ","为什么要演奏春日影","剪切线","soyorin","ba.","贝斯","ba","后悔","愿望"],"40":["椎名立希","椎名立希","椎名","Taki","taki","shiinataki","shiinataki","rikki","Rikki","立希","利息","压力姐","熊猫","大鼓队员","这简直就是我","哈？","哈?","dr.","鼓手","dr","自卑感","未来"]},"bandId":{"0":["混","混活","mix"],"1":["poppin'party","poppinparty","ppp","破琵琶","popipa","poppin","poppinparty","ポピパ","歩品破茶"],"2":["afterglow","ag","夕阳红","悪蓋愚狼"],"3":["ハロー、ハッピーワールド！","hello,happyworld!","hello，happyworld！","hello,happyworld！","hello,happyworld","hhw","harohapi","ハロハピ","破狼法被威悪怒","儿歌团","好好玩"],"4":["pastel＊palettes","pastel*palettes","pastelpalettes","pastel","palettes","pasupare","pp","パスパレ","破巣照破烈斗","怕死怕累"],"5":["roselia","r","r组","ロゼリア","相声团","相声组","露世裏悪","露世里恶"],"6":["其他"],"7":["其他"],"8":["其他"],"9":["其他"],"10":["其他"],"11":["其他"],"12":["其他"],"13":["其他"],"14":["其他"],"15":["其他"],"16":["其他"],"17":["其他"],"18":["raiseasuilen","raise","suilen","ras","ラス","零図悪酔恋","睡莲","麗厨唖睡蓮","睡蓮"],"19":["其他"],"20":["其他"],"21":["morfonica","毛二力","monika","monica","モニカ","蝶团","蝶","m团","m"],"22":["其他"],"23":["其他"],"24":["其他"],"25":["其他"],"26":["其他"],"27":["其他"],"28":["其他"],"29":["其他"],"30":["其他"],"31":["其他"],"32":["其他"],"33":["其他"],"34":["其他"],"35":["其他"],"36":["其他"],"37":["其他"],"38":["其他"],"39":["其他"],"40":["其他"],"41":["其他"],"42":["其他"],"43":["其他"],"44":["其他"],"45":["MyGO!!!!!","MyGO！！！！！","mygo","我去！！！！！","我去!!!!!","我去","卖狗"]},"attribute":{"powerful":["powerful","power","红","红色"],"cool":["cool","蓝","蓝色"],"happy":["happy","橙色","橙","黄"],"pure":["pure","绿","绿色"]},"type":{"permanent":["常驻","无期限"],"event":["活动","活动卡"],"campaign":["联动","选举","联名合作"],"initial":["初始"],"limited":["限定","期间限定"],"dreamfes":["fes限定","fes","梦幻fes限定","梦幻fes","dreamfes","dreamfestival","限定","期间限定","dfes"],"kirafes":["fes限定","fes","闪光fes限定","闪光fes","kirafes","kiramekitival","限定","期间限定","kfes"],"birthday":["生日","birthday","生日限定","限定","期间限定"]},"rarity":{"5":["5x","5星","5*","五星"],"4":["4x","4星","4*","四星"],"3":["3x","3星","3*","三星"],"2":["2x","2星","2*","二星"],"1":["1x","1星","1*","一星"]},"skillType":{"score":["分卡","分","大分卡","大分"],"judge":["判卡","判","判定","判定加强"],"life":["奶卡","奶","治疗","生命恢复","生命回复"],"damage":["盾","无敌","盾卡"]},"eventType":{"story":["一般活动","协力"],"versus":["对邦","竞演","竞演live"],"live_try":["ex","ex牌","live试炼"],"challenge":["cp","cp活","cp活动","挑战live"],"mission_live":["任务","任务live","协力","副队"],"festival":["5v5","5对5","1v9","1v5"],"medley":["组曲","三曲","三组曲"]},"tag":{"anime":["动画","anime","animation"],"tie_up":["翻唱","cover"],"normal":["原创","原唱","original"]},"difficulty":{"0":["easy","简单","简","ez"],"1":["normal","普通","普","nm"],"2":["hard","困难","困","hd"],"3":["expert","专家","专","ex"],"4":["special","特殊","特","sp"]},"server":{"0":["jp","日","日本","日服","日本服"],"1":["en","国际","英","英语","国际服"],"2":["tw","台","台湾","台服","台湾服"],"3":["cn","国","中国","国服","国内服"],"4":["kr","韩","韩国","韩服","韩国服"]}}"""
            .DeserializeJson<Dictionary<string, Dictionary<string, string[]>>>()!;
}
