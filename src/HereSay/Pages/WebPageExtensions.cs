using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HereSay.Pages;
using N2.Web.Parts;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ServiceModel.Syndication;
using System.Web;
using Rolcore.Web;

namespace HereSay.Pages
{
    public static class WebPageExtensions
    {
        public static bool GetExcludeFromFeed(this WebPage webPage)
        {
            return webPage.GetDetail<bool>(
                EditModeFields.Syndication.ExcludeFromFeedName,
                EditModeFields.Syndication.ExcludeFromFeedDefaultValue);
        }

        public static string GetContentForSyndication(this WebPage webPage)
        {
            if (webPage.GetExcludeFromFeed())
                throw new InvalidOperationException("Cannot get content for syndication from a page that has been excluded from syndication.");

            const string syndicationZone = "Content"; //TODO: Figure out some way to make this configurable

            StringBuilder result = new StringBuilder();
            List<N2.ContentItem> contentItems = webPage.GetPublishedChildren<N2.ContentItem>(true)
                .Where(part => 
                    !part.IsPage 
                 && part is IAddablePart 
                 && part.ZoneName == syndicationZone)
                .ToList();

            if (contentItems.Count > 0)
            {
                using (StringWriter resultWriter = new StringWriter(result))
                {
                    using (HtmlTextWriter htmlWrite = new HtmlTextWriter(resultWriter))
                    {
                        PlaceHolder ph = new PlaceHolder();
                        //ph.Controls.Add(new Literal() { Text = "<![CDATA[" });
                        contentItems.ForEach(item => ((IAddablePart)item).AddTo(ph));
                        //ph.Controls.Add(new Literal() { Text = "]]>" });  
                        ph.RenderControl(htmlWrite);
                    }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Converts the given <see cref="WebPage"/> to a <see cref="SyndicationItem"/> by copying
        /// content in the "Content" zone into the resulting <see cref="SyndicationItem.Content"/> 
        /// property.
        /// </summary>
        /// <param name="webPage">Specifies the <see cref="WebPage"/> to convert.</param>
        /// <returns>The specified WebPage represented as a SyndicationItem.</returns>
        public static SyndicationItem ToSyndicationItem(this WebPage webPage)
        {
            string syndicationBody = webPage.GetContentForSyndication();

            //we need to find the "more" tag (if it exists) and extract everything before that for the "Summary"
            Match regexMatch = Regex.Match(syndicationBody, @"<!--[\s]*more[\s]*-->", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            string syndicationSummary = (regexMatch.Success) ? syndicationBody.Substring(0, regexMatch.Index) : String.Empty;
            
            Uri permalink = new Uri(webPage.SafeUrl);

            SyndicationItem result = new SyndicationItem(
                webPage.PageTitle.RemoveHtml(), 
                string.Empty, 
                null, 
                string.Empty, 
                webPage.Updated) { 
                    PublishDate = webPage.GetDetail<DateTime>(EditModeFields.Syndication.ContentPublishDateName, webPage.Published.Value), 
                    Summary = new TextSyndicationContent(syndicationSummary.RemoveHtml(), TextSyndicationContentKind.Plaintext), 
                    Content = SyndicationContent.CreateHtmlContent(syndicationBody) };

            // 
            // This feature is still under development. If issues are found with the content, check
            // out the suggestions on:
            // http://stackoverflow.com/questions/1118409/syndicationfeed-content-as-cdata

            result.AddPermalink(permalink);

            return result;
        }
    }
}
