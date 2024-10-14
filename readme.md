### TODO

- [x] Satori 支持
- [ ] 聊天输出美化（Satori）
- [ ] 上传车牌: 自动检测车牌并上传

### 项目引用

- [Satori.NET](https://github.com/bsdayo/Satori.NET)（[Forked](https://github.com/polyarkk/Satori.NET/tree/main)）
- [Lagrange.Core](https://github.com/LagrangeDev/Lagrange.Core)

### 项目结构：

- `Tsugu.Api`: 后端 API 调用类库
- `Tsugu.Lagrange`: 机器人前端

### 构建

```shell
git clone --recurse-submodules https://github.com/polyarkk/Tsugu.Lagrange
cd Tsugu.Lagrange/Tsugu.Lagrange

# for linux, single file, including runtime
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true

# for windows, single file, including runtime
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```