namespace ffxiv_apps.Common;

public class Link(IConfiguration config)
{
    public string BaseUrl => config.GetValue<string>("AppBasePath")!;

    public string GetHref(string path)
    {
        return "/" + BaseUrl + path.TrimStart('/');
    }
}
