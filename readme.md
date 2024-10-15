# <p align="center">🟢Tsugu.Frontend🟢</p>

### TODO

- [ ] 完善日志输出（文件/控制台）
- [x] Satori 支持
- [ ] 聊天输出美化（Satori）
- [ ] 上传车牌：自动检测车牌并上传

### 项目引用

- [Satori.NET](https://github.com/bsdayo/Satori.NET)（[Forked](https://github.com/polyarkk/Satori.NET/tree/main)）
- [Lagrange.Core](https://github.com/LagrangeDev/Lagrange.Core)

### 项目结构：

- `Tsugu.Api`: 后端 API 调用类库
- `Tsugu.Frontend`: 机器人前端

### 构建

```shell
git clone --recurse-submodules https://github.com/polyarkk/Tsugu.Frontend
cd Tsugu.Frontend/Tsugu.Frontend

# for linux, single file, including runtime
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true

# for windows, single file, including runtime
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### 配置示例

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Trace"
    }
  },
  "Tsugu": {
    "Whitelisted": true,
    "BackendUrl": "http://127.0.0.1:3000",
    "Compress": true,
    "NeedMentioned": true,
    "Groups": [
      "lagrange:red:114514",
      "satori:onebot:114514",
      "satori:kook:114514"
    ],
    "Friends": [
      "lagrange:red:114514",
      "satori:onebot:114514",
      "satori:kook:114514"
    ],
    "Admins": [
      "lagrange:red:114514",
      "satori:onebot:114514",
      "satori:kook:114514"
    ],
    "Lagrange": {
      "Enabled": false
    },
    "Satori": {
      "Enabled": true,
      "Server": "http://47.100.40.118:5140/",
      "Token": "tsugu",
      "Bots": [
        {
          "Platform": "onebot",
          "SelfId": "1919810"
        }
      ]
    }
  }
}
```

### 配置

#### Tsugu 机器人配置 `Tsugu`

> 对于协议名：只有`satori`、`lagrange`
> 
> 对于`lagrange`协议，平台名只能为`red`

- `Whitelisted`：是否启用白名单模式，启用后将只对***群聊只有`Group`中列出的群组***，***私聊只有`Friends`中列出的好友***返回消息。
- `BackendUrl`：Tsugu 后端地址
- `Compress`：是否压缩图片
- `NeedMentioned`：是否必须要被@才会对用户触发指令
- `Groups`：白/黑名单群组，格式为`协议名:平台名:群号`
- `Friends`：白/黑名单私聊用户，格式为`协议名:平台名:用户ID`
- `Admins`：机器人管理员，格式为`协议名:平台名:用户ID`
- `Lagrange`: Lagrange 机器人配置
  - `Enabled`：是否启用 Lagrange 机器人
- `Satori`：Satori 机器人配置
  - `Enabled`：是否启用 Satori 机器人
  - `Server`：Satori 服务器地址
  - `Token`：Satori 服务器 Token，没有可以留空
  - `Bots`: 需要接管的机器人（数组）
    - `Platform`：平台名
    - `SelfId`：机器人 ID
