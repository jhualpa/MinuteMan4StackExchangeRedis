using System.Collections.Generic;
using System.Linq;
using Minuteman.Abstract;
using Minuteman.Redis;
using Ninject;
using StackExchange.Redis;
using Minuteman.Infrastructure.Configuration.Settings;
using Minuteman.Infrastructure.Serialization.Abstract;
using Minuteman.Infrastructure.Serialization;
using Minuteman.Infrastructure.Configuration.Providers;

namespace Minuteman.Tests
{
    class TestModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            // Serializer
            Bind<ISerializer>().To<ServiceStackSerializer>();

            // Configuration
            Bind<IRedisSettings>().To<RedisSettings>();
            Bind<IWebConfigProvider>().To<WebConfigProvider>();

            // Redis client
            Bind<ConnectionMultiplexer>().ToMethod(ctx =>
            {
                var configurationOptions = ConfigurationOptions.Parse(ctx.Kernel.Get<IRedisSettings>().Configuration);
                var connection = ConnectionMultiplexer.Connect(configurationOptions);
                return connection;
            }).InSingletonScope();

            // The database number for this redis instance is 2
            Bind<IDatabase>().ToMethod(ctx => ctx.Kernel.Get<ConnectionMultiplexer>().GetDatabase(2));
            Bind<ISubscriber>().ToMethod(ctx => ctx.Kernel.Get<ConnectionMultiplexer>().GetSubscriber());
            
            var endpoints = Kernel.Get<ConnectionMultiplexer>().GetEndPoints();
            var servers = endpoints.Select(e => Kernel.Get<ConnectionMultiplexer>().GetServer(e)).ToList();
           
            // Minuteman client
            Bind<IClient>().To<Client>().WithConstructorArgument(typeof(IEnumerable<IServer>), servers);
        }
    }
}
