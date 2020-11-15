using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Word2Rtf.Models;
using Word2Rtf.Parsers;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Word2Rtf
{
    public class Function
    {
        public Function()
        {
            object locker = new object();
            lock (locker)
            {
                if (C.Sections == null || C.Books == null)
                {
                    // Set up Dependency Injection
                    var serviceCollection = new ServiceCollection();
                    ConfigureServices(serviceCollection);
                    var serviceProvider = serviceCollection.BuildServiceProvider();
                    var configService = serviceProvider.GetService<IConfigurationService>();
                    var config = configService.GetConfiguration();
                
                    C.Sections = config.GetSection("sections").Get<Section[]>();
                    C.Books = config.GetSection("books").Get<Book[]>();
                }
            }
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public object FunctionHandler(Payload input, ILambdaContext context)
        {
            var json = input.Input.Break().Parse().Combine();
            return json;
        }
        
        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IConfigurationService, ConfigurationService>();
        }
    }
}
