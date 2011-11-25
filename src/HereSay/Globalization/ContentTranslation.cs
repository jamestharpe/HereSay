using N2;
using N2.Engine.Globalization;
using System.Diagnostics.Contracts;
using System;

namespace HereSay.Globalization
{
    public class ContentTranslation
    {

        public ContentTranslation(ContentItem content, ILanguage language)
        {
            if (content == null)
                throw new ArgumentNullException("content", "content is null.");
            if (language == null)
                throw new ArgumentNullException("language", "language is null.");

            this.Page = content;
            this.Language = language;
        }

        public ContentItem Page { get; set; }

        public ILanguage Language { get; set; }
    }
}
