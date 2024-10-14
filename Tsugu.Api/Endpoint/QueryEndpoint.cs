using Tsugu.Api.Entity;
using Tsugu.Api.Enum;
using Tsugu.Api.Misc;

namespace Tsugu.Api.Endpoint;

public class QueryEndpoint(TsuguHttpClient client) {
    /// <summary>
    /// 查询指定服务器的团队 festival 活动舞台数据，仅当指定活动为团队 festival 活动时可以获取到数据。
    /// </summary>
    /// <param name="mainServer">用户所在的服务器</param>
    /// <param name="eventId">指定的活动 ID，不指定则为当前活动。</param>
    /// <param name="meta">是否携带歌曲分数信息。</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    public async Task<string> EventStage(
        Server mainServer, uint? eventId = null, bool meta = false, bool compress = true
    ) {
        Dictionary<string, object> o = new() {
            ["mainServer"] = mainServer,
            ["meta"] = meta,
            ["compress"] = compress,
        };

        if (eventId != null) {
            o["eventId"] = eventId;
        }

        return (await client.TsuguPost("/eventStage", o))[0];
    }

    /// <summary>
    /// 模拟指定卡池的抽卡结果。
    /// </summary>
    /// <param name="mainServer">抽卡模拟使用的服务器，同一卡池在不同服务器的表现可能不同。</param>
    /// <param name="gachaId">若传入则模拟指定卡池的抽卡结果，不传入则默认为指定服务器的最新卡池。</param>
    /// <param name="times">模拟抽卡的次数。</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    public async Task<string> GachaSimulate(
        Server mainServer, uint? gachaId = null, uint times = 10, bool compress = true
    ) {
        Dictionary<string, object> o = new() {
            ["mainServer"] = mainServer,
            ["times"] = times,
            ["compress"] = compress,
        };

        if (gachaId != null) {
            o["gachaId"] = gachaId;
        }

        return (await client.TsuguPost("/gachaSimulate", o))[0];
    }

    /// <summary>
    /// 获取指定 ID 卡牌的卡面图片。
    /// </summary>
    /// <param name="cardId">要获取卡面的卡牌 ID。</param>
    /// <returns>图片 Base64（拥有特训后图片的卡牌将同时返回特训前的特训后的图片）</returns>
    public async Task<string[]> GetCardIllustration(uint cardId) {
        return await client.TsuguPost("/getCardIllustration", new { CardId = cardId });
    }

    /// <summary>
    /// 查询与指定活动相关的指定档位的历史预测线。
    /// </summary>
    /// <param name="mainServer">要查询的指定服务器。</param>
    /// <param name="tier">要查询的档位排名。</param>
    /// <param name="eventId">若传入则查询指定活动，不传入则默认为指定服务器的最新活动。</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    public async Task<string> CutoffListOfRecentEvent(
        Server mainServer, uint tier, uint? eventId = null, bool compress = true
    ) {
        Dictionary<string, object> o = new() {
            ["mainServer"] = mainServer,
            ["tier"] = tier,
            ["compress"] = compress,
        };

        if (eventId != null) {
            o["eventId"] = eventId;
        }

        return (await client.TsuguPost("/cutoffListOfRecentEvent", o))[0];
    }

    /// <summary>
    /// 传入获取到的房间列表信息，获取绘制后的图片，或当没有房间时返回的信息。
    /// </summary>
    /// <param name="roomList">房间信息</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    /// <exception cref="EndpointCallException">若传入的房间信息中没有房间，则会得到消息为“myc”的报错</exception>
    public async Task<string> RoomList(Room[] roomList, bool compress = true) {
        if (roomList.Length == 0) {
            throw new EndpointCallException("myc");
        }

        return (await client.TsuguPost("/roomList", new { RoomList = roomList, Compress = compress }))[0];
    }

    /// <summary>
    /// 查询指定卡牌的信息，或查询符合条件的卡牌列表。
    /// </summary>
    /// <param name="displayedServerList">默认服务器列表，将会按顺序查询第一个有效的服务器。</param>
    /// <param name="text">查询文本，若为数字则尝试返回特定卡牌ID的卡牌信息，否则尝试返回当前查询条件下的卡牌列表。</param>
    /// <param name="useEasyBg">是否使用简易背景。false 即为不使用简易背景，此时后端图片生成耗时可能会大幅增加。</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    /// <exception cref="EndpointCallException">查询文本为空时将抛出异常</exception>
    public async Task<string> SearchCard(
        Server[] displayedServerList, string text, bool useEasyBg = false, bool compress = true
    ) {
        Dictionary<string, object> o = new() {
            ["displayedServerList"] = displayedServerList,
            ["useEasyBG"] = useEasyBg,
            ["compress"] = compress,
        };

        if (string.IsNullOrWhiteSpace(text)) {
            throw new EndpointCallException($"{nameof(text)} 不得为空");
        }

        if (int.TryParse(text, out _)) {
            o["text"] = text;
        } else {
            o["fuzzySearchResult"] = FuzzySearch.Parse(text.Split(" "));
        }

        return (await client.TsuguPost("/searchCard", o))[0];
    }

    /// <summary>
    /// 查询符合条件的角色信息。
    /// </summary>
    /// <param name="displayedServerList">默认服务器列表，将会按顺序查询第一个有效的服务器。</param>
    /// <param name="text">查询文本</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    /// <exception cref="EndpointCallException">查询文本为空时将抛出异常</exception>
    public async Task<string> SearchCharacter(Server[] displayedServerList, string text, bool compress = true) {
        Dictionary<string, object> o = new() {
            ["displayedServerList"] = displayedServerList,
            ["compress"] = compress,
        };

        if (string.IsNullOrWhiteSpace(text)) {
            throw new EndpointCallException($"{nameof(text)} 不得为空");
        }

        if (int.TryParse(text, out _)) {
            o["text"] = text;
        } else {
            o["fuzzySearchResult"] = FuzzySearch.Parse(text.Split(" "));
        }

        return (await client.TsuguPost("/searchCharacter", o))[0];
    }

    /// <summary>
    /// 查询指定活动的信息，或查询符合条件的活动列表。
    /// </summary>
    /// <param name="displayedServerList">默认服务器列表，将会按顺序查询第一个有效的服务器。</param>
    /// <param name="text">查询文本，若为数字则尝试返回特定活动ID的活动信息，否则尝试返回当前查询条件下的活动列表。</param>
    /// <param name="useEasyBg">是否使用简易背景。false 即为不使用简易背景，此时后端图片生成耗时可能会大幅增加。</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    /// <exception cref="EndpointCallException">查询文本为空时将抛出异常</exception>
    public async Task<string> SearchEvent(
        Server[] displayedServerList, string text, bool useEasyBg = false, bool compress = true
    ) {
        Dictionary<string, object> o = new() {
            ["displayedServerList"] = displayedServerList,
            ["useEasyBG"] = useEasyBg,
            ["compress"] = compress,
        };

        if (string.IsNullOrWhiteSpace(text)) {
            throw new EndpointCallException($"{nameof(text)} 不得为空");
        }

        if (int.TryParse(text, out _)) {
            o["text"] = text;
        } else {
            o["fuzzySearchResult"] = FuzzySearch.Parse(text.Split(" "));
        }

        return (await client.TsuguPost("/searchEvent", o))[0];
    }

    /// <summary>
    /// 查询指定卡池的信息。
    /// </summary>
    /// <param name="displayedServerList">默认服务器列表，将会按顺序查询第一个有效的服务器。</param>
    /// <param name="gachaId">查询的卡池 ID。</param>
    /// <param name="useEasyBg">是否使用简易背景。false 即为不使用简易背景，此时后端图片生成耗时可能会大幅增加。</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    public async Task<string> SearchGacha(
        Server[] displayedServerList, uint gachaId, bool useEasyBg = false, bool compress = true
    ) {
        return (await client.TsuguPost("/searchGacha",
            new {
                DisplayedServerList = displayedServerList,
                GachaId = gachaId,
                UseEasyBG = useEasyBg,
                Compress = compress,
            }
        ))[0];
    }

    /// <summary>
    /// 查询指定服务器的指定玩家的状态图片。仅能查询到已公开的信息。
    /// </summary>
    /// <param name="playerId">查询的玩家 ID。</param>
    /// <param name="mainServer">玩家所在的服务器。</param>
    /// <param name="useEasyBg">是否使用简易背景。false 即为不使用简易背景，此时后端图片生成耗时可能会大幅增加。</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    public async Task<string> SearchPlayer(
        uint playerId, Server mainServer, bool useEasyBg = false, bool compress = true
    ) {
        return (await client.TsuguPost("/searchPlayer",
            new {
                PlayerId = playerId,
                MainServer = mainServer,
                UseEasyBG = useEasyBg,
                Compress = compress,
            }
        ))[0];
    }

    /// <summary>
    /// 查询指定歌曲的信息，或查询符合条件的歌曲列表。
    /// </summary>
    /// <param name="displayedServerList">默认服务器列表，将会按顺序查询第一个有效的服务器。</param>
    /// <param name="text">查询参数，若为数字则尝试返回特定歌曲ID的卡牌信息，否则尝试返回当前查询条件下的歌曲列表。</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    /// <exception cref="EndpointCallException">查询文本为空时将抛出异常</exception>
    public async Task<string> SearchSong(Server[] displayedServerList, string text, bool compress = true) {
        Dictionary<string, object> o = new() {
            ["displayedServerList"] = displayedServerList,
            ["compress"] = compress,
        };

        if (string.IsNullOrWhiteSpace(text)) {
            throw new EndpointCallException($"{nameof(text)} 不得为空");
        }

        if (int.TryParse(text, out _)) {
            o["text"] = text;
        } else {
            o["fuzzySearchResult"] = FuzzySearch.Parse(text.Split(" "));
        }

        return (await client.TsuguPost("/searchSong", o))[0];
    }

    /// <summary>
    /// 获取指定歌曲的谱面图片。
    /// </summary>
    /// <param name="displayedServerList">默认服务器列表，将会按顺序查询第一个有效的服务器。</param>
    /// <param name="songId">指定的歌曲 ID。</param>
    /// <param name="difficulty">指定谱面难度。</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    public async Task<string> SongChart(
        Server[] displayedServerList, uint songId, ChartDifficulty difficulty, bool compress = true
    ) {
        return (await client.TsuguPost("/songChart",
            new {
                DisplayedServerList = displayedServerList,
                SongId = songId,
                DifficultyId = (uint)difficulty,
                Compress = compress,
            }
        ))[0];
    }

    /// <summary>
    /// 查询歌曲分数排行表。
    /// </summary>
    /// <param name="displayedServerList">默认服务器列表，将会按顺序查询第一个有效的服务器。</param>
    /// <param name="mainServer">要查询的主服务器。</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    public async Task<string> SongMeta(
        Server[] displayedServerList, Server mainServer, bool compress = true
    ) {
        return (await client.TsuguPost("/songMeta",
            new {
                DisplayedServerList = displayedServerList,
                MainServer = mainServer,
                Compress = compress,
            }
        ))[0];
    }

    /// <summary>
    /// 查询指定活动的指定档位的预测线。
    /// </summary>
    /// <param name="mainServer">要查询的指定服务器。</param>
    /// <param name="tier">要查询的档位排名。</param>
    /// <param name="eventId">若传入则查询指定活动，不传入则默认为指定服务器的最新活动。</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    public async Task<string> CutoffDetail(
        Server mainServer, uint tier, uint? eventId = null, bool compress = true
    ) {
        Dictionary<string, object> o = new() {
            ["mainServer"] = mainServer,
            ["tier"] = tier,
            ["compress"] = compress,
        };

        if (eventId != null) {
            o["eventId"] = eventId;
        }

        return (await client.TsuguPost("/cutoffDetail", o))[0];
    }

    /// <summary>
    /// 查询指定活动的全档位预测线。
    /// </summary>
    /// <param name="mainServer">要查询的指定服务器。</param>
    /// <param name="eventId">若传入则查询指定活动，不传入则默认为指定服务器的最新活动。</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    public async Task<string> CutoffAll(
        Server mainServer, uint? eventId = null, bool compress = true
    ) {
        Dictionary<string, object> o = new() {
            ["mainServer"] = mainServer,
            ["compress"] = compress,
        };

        if (eventId != null) {
            o["eventId"] = eventId;
        }

        return (await client.TsuguPost("/cutoffAll", o))[0];
    }

    /// <summary>
    /// 根据关键词随机获取一首歌曲的基础信息
    /// </summary>
    /// <param name="mainServer">要查询的指定服务器。</param>
    /// <param name="text">查询参数</param>
    /// <param name="compress">是否在后端压缩图像，压缩图像可以加快传输速度，但是会降低图片清晰度。</param>
    /// <returns>图片 Base64</returns>
    public async Task<string> SongRandom(Server mainServer, string text = "", bool compress = true) {
        return (await client.TsuguPost("/songRandom",
            new {
                MainServer = mainServer,
                Text = text,
                Compress = compress,
            }
        ))[0];
    }
}
