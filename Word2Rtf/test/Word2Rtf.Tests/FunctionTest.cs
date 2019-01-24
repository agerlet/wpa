using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using Word2Rtf;
using System.Reflection;
using System.IO;
using System.Text;

namespace Word2Rtf.Tests
{
    public class FunctionTest
    {
        ILambdaSerializer _jsonSerializer = new Amazon.Lambda.Serialization.Json.JsonSerializer();

        [Fact]
        public async void TestToUpperFunction()
        {
            string input = await LoadAsync("samples/sample-0.txt");
            string snap = await LoadAsync("snaps/snap-0.json");

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var json = function.FunctionHandler(new Models.Payload { Input = input }, context);
            string actual = await GetJsonString(_jsonSerializer, json);

            object t = GetJsonObject(_jsonSerializer, snap);
            string expected = await GetJsonString(_jsonSerializer, t);
            
            Assert.Equal(expected, actual);
        }

        private async Task<string> LoadAsync(string filename)
        {
            string content = null;
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), filename);
            using (var file = new System.IO.StreamReader(path))
            {
                content = await file.ReadToEndAsync();
            }
            return content;
        }

        private async Task<string> GetJsonString(ILambdaSerializer serializer, object jsonObject)
        {
            string jsonString = null;
            using (Stream stream = new MemoryStream())
            {
                serializer.Serialize(jsonObject, stream);
                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream))
                {
                    jsonString = await reader.ReadToEndAsync();
                }
            }
            return jsonString;
        }

        private object GetJsonObject(ILambdaSerializer serializer, string snap)
        {
            object t;
            using(Stream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(snap)))
            {
                t = serializer.Deserialize<object>(stream);
            }
            return t;
        }
    }
}
