using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Word2Rtf.Analysers;
using Word2Rtf.Mixers;
using Word2Rtf.Models;
using Word2Rtf.Parsers;

namespace Word2Rtf
{
    public static class DependencyInjection
    {
        public static IConfigurationBuilder AddConfiguration(this IConfigurationBuilder config)
        {
            return config
                .SetBasePath(Environment.CurrentDirectory)        
                .AddJsonFile("dictionary/sections.json", optional: false, reloadOnChange: true)
                .AddJsonFile("dictionary/book-names.json", false, true)
                ;
        }

        public static void AddDependencyInjections(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services
                .Configure<SectionsSettings>(configuration)
                .Configure<BooksSettings>(configuration)
                .AddSingleton<Bible>()
                .AddSingleton<BibleAnalyser>()
                .AddSingleton<IMixer, EqualLengthMixer>()
                .AddSingleton<IMixer, SingleVerseNumberMixer>()
                .AddSingleton<IMixer, LastMixer>()
                .AddSingleton<MixerFactory>()
                .AddSingleton<ParserHandler>()
                ;
        }
    }
}