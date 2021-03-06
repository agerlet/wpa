using System.Collections.Generic;
using System.Linq;
using Amazon.Lambda.Core;

using Word2Rtf.Models;

namespace Word2Rtf
{
    internal static class Combiner
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public static object Combine(this IEnumerable<Element> elements) => new
        {
            program = elements.Select(element => new
            {
                verses = element.Verses,
            })
        };
    }
}