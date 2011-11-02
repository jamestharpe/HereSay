using N2;
using N2.Engine.Globalization;
using System.Diagnostics.Contracts;
using System;

namespace HereSay.Globalization
{
    public class ContentTranslation
    {
        private ContentItem _Content;
        private ILanguage _Language;

        public ContentTranslation(ContentItem content, ILanguage language)
        {
            if (content == null)
                throw new ArgumentNullException("content", "content is null.");
            if (language == null)
                throw new ArgumentNullException("language", "language is null.");

            this._Content = content;
            this._Language = language;
        }

        public ContentItem Page
        {
            get { return _Content; }
            set { _Content = value; }
        }

        public ILanguage Language
        {
            get { return _Language; }
            set { _Language = value; }
        }
    }
}
