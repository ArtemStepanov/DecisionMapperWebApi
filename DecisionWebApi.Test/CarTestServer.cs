using DecisionWebApi.DbWorkers;
using DecisionWebApi.Models;
using DecisionWebApi.Services;
using DecisionWebApi.Test.DbWorkers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DecisionWebApi.Test.Servers
{
    public class CarTestServer : TestServer
    {
        private CarTestServer(IWebHostBuilder builder) : base(builder)
        {
        }

        public static CarTestServer CreateNew()
        {
            var webHostBuilder =
                new WebHostBuilder()
                    .ConfigureServices(services => { services.AddSingleton<IDbWorker<Car>, ListDbWorker<Car>>(); })
                    .UseStartup<Startup>();

            return new CarTestServer(webHostBuilder);
        }
    }
}