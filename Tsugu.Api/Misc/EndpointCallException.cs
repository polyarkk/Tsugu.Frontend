namespace Tsugu.Api.Misc;

/// <summary>
/// 以下情况可能会抛出此异常：
/// <list type="bullet">
/// <item>无法连接到后端</item>
/// <item>后端异常</item>
/// <item>返回图片的接口返回的不是图片</item>
/// <item>返回其他数据的接口返回的<c>Status</c>字段为<c>failed</c></item>
/// </list>
/// </summary>
public class EndpointCallException : Exception {
    public EndpointCallException(string? msg) : base(msg ?? "服务器异常") { }
}
