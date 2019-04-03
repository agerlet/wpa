using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using System.IO;

namespace Word2Rtf.Tests
{
    public class FunctionTest
    {
        readonly ILambdaSerializer _jsonSerializer;
        readonly Function _function;
        readonly TestLambdaContext _context;

        public FunctionTest()
        {
            _jsonSerializer = new Amazon.Lambda.Serialization.Json.JsonSerializer();
            _function = new Function();
            _context = new TestLambdaContext();
        }

        [Fact]
        public async void Snap_0_TestFunction()
        {
            string input = await "samples/sample-0.txt".LoadAsync();
            // Invoke the lambda function and confirm the string was upper cased.
            var json = _function.FunctionHandler(new Models.Payload { Input = input }, _context);
            string actual = await _jsonSerializer.GetJsonString(json);

            string snap = await "snaps/snap-0.json".LoadAsync();
            object o = _jsonSerializer.GetJsonObject(snap);
            string expected = await _jsonSerializer.GetJsonString(o);
            
            Assert.Equal(expected, actual);
        }

        //[Fact]
        public async void Snap_1_TestFunction()
        {
            string input = await "samples/sample-1.txt".LoadAsync();
            // Invoke the lambda function and confirm the string was upper cased.
            var json = _function.FunctionHandler(new Models.Payload { Input = input }, _context);
            string actual = await _jsonSerializer.GetJsonString(json);

            string snap = await "snaps/snap-1.json".LoadAsync();
            object o = _jsonSerializer.GetJsonObject(snap);
            string expected = await _jsonSerializer.GetJsonString(o);
            
            Assert.Equal(expected, actual);
        }
    }
}
