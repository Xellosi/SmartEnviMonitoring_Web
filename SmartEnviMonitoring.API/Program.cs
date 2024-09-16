using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using MQTTnet;
using MQTTnet.AspNetCore;
using MQTTnet.Server;
using Serilog;
using SmartEnviMonitoring.Data;
using SmartEnviMonitoring.API.Configurations;
using SmartEnviMonitoring.API.Data.System;
using SmartEnviMonitoring.API.Repositories;
using SmartEnviMonitoring.API.Hubs;
using SmartEnviMonitoring.API.Data.Communication;
using SmartEnviMonitoring.API.Services;
using System.Diagnostics.CodeAnalysis;
using SmartEnviMonitoring.Common.Clients;


namespace SmartEnviMonitoring.API;

class Program
{
    private static MqttManager _mqttManager;
    private static MqttServer _mqttServer;
    static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
        .WriteTo.Console().CreateLogger();

        Log.Information("Starting up!");

        try{
            var options = new MqttServerOptionsBuilder().WithDefaultEndpoint();
            _mqttServer = new MqttFactory().CreateMqttServer(options.Build());
            _mqttManager = new MqttManager(_mqttServer);
            _mqttServer.StartAsync().Wait();
        }
        catch(Exception exc){
            Log.Error(exc, "starting mqtt server failed.");
            return;
        }

        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAutoMapper(typeof(MapperConfig));
            
            builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
            builder.Services.AddScoped<ISystemEventRepository, SystemEventRepository>();
            builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
            builder.Services.AddScoped<IAudioRecordRepository, AudioRecordRepository>();
            builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();

            builder.Services.AddSingleton<ILoginDevicesService, LoginDevicesService>();
            builder.Services.AddSingleton<MqttManager>(_mqttManager);
            builder.Services.AddSingleton(TimeProvider.System);
            
            builder.Services.AddScoped<HttpResBuilder>();

            string? connString = builder.Configuration.GetConnectionString("AppDbConnection");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connString));
            
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            IdentityBuilder idbuilder = builder.Services.AddIdentityCore<ApiUser>();
            idbuilder = new IdentityBuilder(idbuilder.UserType, idbuilder.Services);
            idbuilder.AddEntityFrameworkStores<AppDbContext>();
            idbuilder.AddSignInManager<SignInManager<ApiUser>>();

            builder.Host.UseSerilog((ctx, lc) =>
                lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

            builder.Services.AddHostedService<LifetimeEventsHostedService>();

            builder.Services.AddSignalR();

            // builder.Services.AddCors(options => {
            //     options.AddPolicy("CorsPolicy",
            //     policy => {
            //         policy.WithOrigins("http://localhost:5276")
            //         .AllowAnyHeader()
            //         .AllowAnyMethod()
            //         .AllowCredentials();
            //     });
            // });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()){
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            
            //app.UseAuthentication();
            //app.UseAuthorization();

            app.MapControllers();

            app.UseCors(cors => cors
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)
            .AllowCredentials());

            //app.UseEndpoints(endpoints => endpoints.MapHub<Hubs.ChatHub>("/chathub"));
            app.MapHub<DeviceHub>(DeviceHubClient.HUBURL);

            using(IServiceScope serviceScope = app.Services.CreateScope()){
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();
            }

            app.Run();
        }
        catch (Exception ex) when (ex is not HostAbortedException && ex.Source != "Microsoft.EntityFrameworkCore.Design")
        {
            Log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
            return;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static Thread StartMQTTServer(){
        Thread thrd = new Thread(()=>{
            var options = new MqttServerOptionsBuilder().WithDefaultEndpoint();
            MqttServer server = new MqttFactory().CreateMqttServer(options.Build());
            server.StartAsync().Wait();
        });
        thrd.Start();
        return thrd;
    }

    private static Task StartMqttHeartBeatTask(MqttServer server)
    {
        return Task.Run(async () =>
        {
            var mqttApplicationMessage = new MqttApplicationMessageBuilder()
            .WithPayload($"Test application message from MQTTnet server.")
            .WithTopic("message")
            .Build();

            while (true)
            {
                try
                {
                    await server.InjectApplicationMessage(
                        new InjectedMqttApplicationMessage(mqttApplicationMessage)
                        {
                            SenderClientId = "server"
                        });
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        });
    }
}