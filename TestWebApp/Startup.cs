using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using CustomFollowerGoal.Code;
using CustomFollowerGoal.Code.HostedServices.Scheduler;
using CustomFollowerGoal.Code.HostedServices.Scheduler.SchedulerTasks;
using CustomFollowerGoal.Hubs;
using CustomFollowerGoal.Models.WebHooks;
using CustomFollowerGoal.Code.UserAccessToken;

namespace CustomFollowerGoal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddControllersWithViews().AddNewtonsoftJson();

            //add user access state store
            services.AddSingleton<UserAccessStateStore, UserAccessStateStore>(serviceProvider =>
            {
                return new UserAccessStateStore();
            });

            //add user access token store
            services.AddSingleton<UserAccessTokenStore, UserAccessTokenStore>(serviceProvider =>
            {
                return new UserAccessTokenStore();
            });

            //add twitch api client
            services.AddSingleton<ITwitchApiClient, TwitchApiClient>(serviceProvider =>
            {
                IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
                IConfigurationSection appSettings = configuration.GetSection("AppSettings");

                string clientId = appSettings["api-client-id"];
                string clientSecret = appSettings["api-client-secret"];
                return new TwitchApiClient(clientId, clientSecret);
            });

            //add scheduled webhook subscription task for follows
            services.AddSingleton<IScheduledTask, WebHookSubscriptionTask>(serviceProvider =>
            {
                ITwitchApiClient twitchApiClient = serviceProvider.GetService<ITwitchApiClient>();
                UserAccessTokenStore userAccessTokenStore = serviceProvider.GetService<UserAccessTokenStore>();
                var model = new WebHooksModel()
                {
                    Callback = "http://test-env.eba-aafhtxzp.us-west-2.elasticbeanstalk.com/api/twitchwebhook/follows",
                    Mode = "subscribe",
                    Topic = "https://api.twitch.tv/helix/users/follows?first=1&to_id=58669321",
                    LeaseSeconds = 864000 //max lease time
                };

                return new WebHookSubscriptionTask(twitchApiClient, model, userAccessTokenStore);
            });

            //add scheduled webhook subscription task for subs
            services.AddSingleton<IScheduledTask, WebHookSubscriptionTask>(serviceProvider =>
            {
                ITwitchApiClient twitchApiClient = serviceProvider.GetService<ITwitchApiClient>();
                UserAccessTokenStore userAccessTokenStore = serviceProvider.GetService<UserAccessTokenStore>();
                var model = new WebHooksModel()
                {
                    Callback = "http://test-env.eba-aafhtxzp.us-west-2.elasticbeanstalk.com/api/twitchwebhook/subs",
                    Mode = "subscribe",
                    Topic = "https://api.twitch.tv/helix/subscriptions/events?broadcaster_id=58669321&first=1",
                    LeaseSeconds = 864000 //max lease time
                };

                return new WebHookSubscriptionTask(twitchApiClient, model, userAccessTokenStore);
            });

            //add scheduled followers update task
            services.AddSingleton<IScheduledTask, UpdateFollowersTask>(serviceProvider =>
            {
                IHubContext<FollowersHub> hubContext = serviceProvider.GetService<IHubContext<FollowersHub>>();
                ITwitchApiClient twitchApiClient = serviceProvider.GetService<ITwitchApiClient>();
                UserAccessTokenStore userAccessTokenStore = serviceProvider.GetService<UserAccessTokenStore>();
                

                IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
                IConfigurationSection appSettings = configuration.GetSection("AppSettings");

                return new UpdateFollowersTask(hubContext, twitchApiClient, userAccessTokenStore, int.Parse(appSettings["stream-id"]));
            });

            //add scheduled subs update task
            services.AddSingleton<IScheduledTask, UpdateSubsTask>(serviceProvider =>
            {
                ITwitchApiClient twitchApiClient = serviceProvider.GetService<ITwitchApiClient>();
                IHubContext<SubsHub> hubContext = serviceProvider.GetService<IHubContext<SubsHub>>();
                UserAccessTokenStore userAccessTokenStore = serviceProvider.GetService< UserAccessTokenStore>();

                IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
                IConfigurationSection appSettings = configuration.GetSection("AppSettings");

                return new UpdateSubsTask(hubContext, twitchApiClient, userAccessTokenStore, int.Parse(appSettings["stream-id"]));
            });

            //add scheduled task - refresh user access token
            services.AddSingleton<IScheduledTask, RefreshUserAccessTokenTask>(serviceProvider =>
            {
                ITwitchApiClient twitchApiClient = serviceProvider.GetService<ITwitchApiClient>();
                UserAccessTokenStore userAccessTokenStore = serviceProvider.GetService<UserAccessTokenStore>();

                return new RefreshUserAccessTokenTask(twitchApiClient, userAccessTokenStore);
            });

            //add scheduler
            services.AddSingleton<IHostedService, SchedulerService>(serviceProvider =>
            {
                var instance = new SchedulerService(serviceProvider.GetServices<IScheduledTask>());
                instance.UnobservedTaskException += (sender, args) =>
                {
                    Console.WriteLine(args.Exception.Message);
                    args.SetObserved();
                };

                return instance;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseWebSockets();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Followers}/{action=Index}/{id?}");

                endpoints.MapHub<FollowersHub>("followersHub");
                endpoints.MapHub<SubsHub>("subsHub");
            });
        }
    }
}
