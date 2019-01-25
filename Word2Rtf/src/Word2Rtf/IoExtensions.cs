using System.IO;
using System.Threading.Tasks;

namespace Word2Rtf
{
    public static class IoExtensions
    {
        public static async Task<string> LoadAsync(this string filename)
        {
            string content = null;
            var path = Path.Combine(Directory.GetCurrentDirectory(), filename);
            using (var file = new StreamReader(path))
            {
                content = await file.ReadToEndAsync();
            }
            return content;
        }
    }
}