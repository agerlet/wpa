using System;

namespace Word2Rtf.Exceptions
{
    public class ImbalancedLanguagesException : Exception
    {
        private const string NotInPair = "中英文沒有成對";

        public ImbalancedLanguagesException() : base(NotInPair)
        {
        }

        public ImbalancedLanguagesException(string input) : base($"{NotInPair}: \"{input}\"")
        {
        }
    }
}