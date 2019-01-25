using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Word2Rtf
{
    public static class JsonExtensions
    {
        public static async Task<string> GetJsonString(this ILambdaSerializer serializer, object jsonObject)
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

        public static object GetJsonObject(this ILambdaSerializer serializer, string snap)
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