using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Word2Rtf.Models;
using Word2Rtf.Parsers;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Word2Rtf
{
    public class Function
    {
        private IConfigurationRoot _configuration;

        /// <summary>
        /// The function to convert the input payload into sections in json
        /// </summary>
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public object FunctionHandler(Payload input, ILambdaContext context)
        {
            var host = Host
                    .CreateDefaultBuilder()
                    .ConfigureAppConfiguration((_, config) =>
                        _configuration = config.AddConfiguration().Build())
                    .ConfigureServices(services =>
                        services.AddDependencyInjections(_configuration))
                    .Build()
                    ;

            using var serviceScope = host.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var handler = serviceProvider.GetService<ParserHandler>();
            var elements = handler.Parse(input.Input.Break());
            var json = elements.Combine();
            return json;
        }
    }
}
