using System.Net;
using ServiceStack;
using ServiceStack.Auth;

[assembly: HostingStartup(typeof(Chat.AppHost))]

namespace Chat;

public class AppHost() : AppHostBase("Chat"), IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) => {
            // Configure ASP.NET Core IOC Dependencies
            services.AddSingleton<IChatHistory, MemoryChatHistory>();
            services.AddPlugin(new ServerEventsFeature());
        });
    
    public override void Configure()
    {
        SetConfig(new HostConfig
        {
            DefaultContentType = MimeTypes.Json,
            AllowSessionIdsInHttpParams = true,
        });

        this.CustomErrorHttpHandlers.Remove(HttpStatusCode.Forbidden);

        //Register all Authentication methods you want to enable for this web app.            
        Plugins.Add(new AuthFeature(() => new AuthUserSession(), [
            new TwitterAuthProvider(AppSettings),   //Sign-in with Twitter
            new FacebookAuthProvider(AppSettings),  //Sign-in with Facebook
            new GithubAuthProvider(AppSettings)     //Sign-in with GitHub
        ]));

        // for lte IE 9 support + allow connections from local web dev apps
        Plugins.Add(new CorsFeature(
            allowOriginWhitelist: ["http://localhost", "http://127.0.0.1:8080", "http://localhost:8080", "http://localhost:8081", "http://null.jsbin.com"],
            allowCredentials: true,
            allowedHeaders: "Content-Type, Allow, Authorization"));
    }
}
