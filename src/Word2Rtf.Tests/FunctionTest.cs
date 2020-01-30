using Xunit;
using Amazon.Lambda.TestUtilities;
using System;

namespace Word2Rtf.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async void Snap_0_Title_Only()
        {
            var jsonSerializer = new Amazon.Lambda.Serialization.Json.JsonSerializer();
            string input = await "samples/sample-0.txt".LoadAsync();
            // Invoke the lambda function and confirm the string was upper cased.
            var json = new Function().FunctionHandler(new Models.Payload { Input = input }, new TestLambdaContext());
            string actual = await jsonSerializer.GetJsonString(json);

            string snap = await "snaps/snap-0.json".LoadAsync();
            object o = jsonSerializer.GetJsonObject(snap.Replace("\\n", Environment.NewLine));
            string expected = await jsonSerializer.GetJsonString(o);
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void Snap_1_Title_With_Bible_Verses()
        {
            var jsonSerializer = new Amazon.Lambda.Serialization.Json.JsonSerializer();
            string input = await "samples/sample-1.txt".LoadAsync();
            // Invoke the lambda function and confirm the string was upper cased.
            var json = new Function().FunctionHandler(new Models.Payload { Input = input }, new TestLambdaContext());
            string actual = await jsonSerializer.GetJsonString(json);

            string snap = await "snaps/snap-1.json".LoadAsync();
            object o = jsonSerializer.GetJsonObject(snap.Replace("\\n", Environment.NewLine));
            string expected = await jsonSerializer.GetJsonString(o);
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void Snap_2_Title_Bible_Verse_and_One_Paragraph_of_Lyrics()
        {
            var jsonSerializer = new Amazon.Lambda.Serialization.Json.JsonSerializer();
            string input = await "samples/sample-2.txt".LoadAsync();
            // Invoke the lambda function and confirm the string was upper cased.
            var json = new Function().FunctionHandler(new Models.Payload { Input = input }, new TestLambdaContext());
            string actual = await jsonSerializer.GetJsonString(json);

            string snap = await "snaps/snap-2.json".LoadAsync();
            object o = jsonSerializer.GetJsonObject(snap.Replace("\\n", Environment.NewLine));
            string expected = await jsonSerializer.GetJsonString(o);
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void Snap_2_1_Title_Bible_Verse_and_Multiple_Paragraphs_of_Lyrics()
        {
            var jsonSerializer = new Amazon.Lambda.Serialization.Json.JsonSerializer();
            string input = await "samples/sample-2.1.txt".LoadAsync();
            // Invoke the lambda function and confirm the string was upper cased.
            var json = new Function().FunctionHandler(new Models.Payload { Input = input }, new TestLambdaContext());
            string actual = await jsonSerializer.GetJsonString(json);

            string snap = await "snaps/snap-2.1.json".LoadAsync();
            object o = jsonSerializer.GetJsonObject(snap.Replace("\\n", Environment.NewLine));
            string expected = await jsonSerializer.GetJsonString(o);
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public async void Snap_3_Title_Bible_Verse()
        {
            var jsonSerializer = new Amazon.Lambda.Serialization.Json.JsonSerializer();
            string input = await "samples/sample-3.txt".LoadAsync();
            // Invoke the lambda function and confirm the string was upper cased.
            var json = new Function().FunctionHandler(new Models.Payload { Input = input }, new TestLambdaContext());
            string actual = await jsonSerializer.GetJsonString(json);

            string snap = await "snaps/snap-3.json".LoadAsync();
            object o = jsonSerializer.GetJsonObject(snap.Replace("\\n", Environment.NewLine));
            string expected = await jsonSerializer.GetJsonString(o);
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public async void Snap_9_Responsive_Reading()
        {
            var jsonSerializer = new Amazon.Lambda.Serialization.Json.JsonSerializer();
            string input = await "samples/sample-9.txt".LoadAsync();
            // Invoke the lambda function and confirm the string was upper cased.
            var json = new Function().FunctionHandler(new Models.Payload { Input = input }, new TestLambdaContext());
            string actual = await jsonSerializer.GetJsonString(json);

            string snap = await "snaps/snap-9.json".LoadAsync();
            object o = jsonSerializer.GetJsonObject(snap.Replace("\\n", Environment.NewLine));
            string expected = await jsonSerializer.GetJsonString(o);
            
            Assert.Equal(expected, actual);
        }
    }
}
