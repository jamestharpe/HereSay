using System.Collections.Generic;
using System.Linq;
using N2.Edit.Workflow;
using HereSay.Pages;
using N2;
namespace HereSay.Parts
{
    [N2.PartDefinition(
        Title = "Search Results",
        Name = "SearchResults", // compatability issue with legacy items
        IconUrl = "~/N2/Resources/icons/folder_explore.png"),
     N2.Web.UI.TabContainer(
         EditModeTabs.Content,
         EditModeTabs.Content,
         EditModeTabs.ContentSortOrder),
     N2.Integrity.RestrictParents(typeof(WebPage)),
     N2.Integrity.RestrictChildren(N2.Integrity.AllowedTypes.None)]
    public class SearchResults : ThemedWebPart
    {
        private string _LastSearchText = null;
        private IEnumerable<WebPage> _Results;

        [N2.Details.EditableTextBox("Parameter Name", 100, ContainerName = EditModeTabs.Content)]
        public string QueryStringParameterName
        {
            get { return GetDetail<string>("QueryStringParameterName", "q"); }
            set { SetDetail<string>("QueryStringParameterName", value); }
        }

        public string SearchText{
            get
            {
                string result = System.Web.HttpContext.Current.Request.QueryString[this.QueryStringParameterName];
                if(result == null)
                    return result;
                return System.Web.HttpContext.Current.Server.UrlDecode(result);
            }
        }

        public IEnumerable<WebPage> Results
        {
            get
            {
                if ((this._Results != null) && (this._LastSearchText == this.SearchText))
                    return this._Results;

                this._LastSearchText = this.SearchText;

                if (!string.IsNullOrEmpty(this._LastSearchText))
                {
                    string like = string.Format("%{0}%", this._LastSearchText);

                    IList<N2.ContentItem> itemResults = N2.Find.Items
                        .Where
                            .OpenBracket()
                                .Title.Like(like)
                                .Or.Name.Like(like)
                                .Or.Detail().Like(like)
                            .CloseBracket()
                            .And.State.Eq(ContentState.Published)
                            .And.Expires.IsNull()
                        .Select();

                    List<WebPage> result = new List<WebPage>(itemResults.Count);

                    //Need to get the LanguageCode (if it exists) so the results can be filtered
                    IEnumerable<LanguageHomePage> langHomePages = this.GetPublishedParents<LanguageHomePage>(true);
                    string languageCode = string.Empty;
                    if(langHomePages.Count() > 0)
                        languageCode = langHomePages.First().LanguageCode.ToLower();

                    bool siteContainsLanguageUrls = N2.Find.Items.Where.Name.Eq(languageCode).Count() > 0;

                    foreach (N2.ContentItem item in itemResults)
                    {
                        WebPage page = (item as WebPage) ?? item.GetPublishedParents<WebPage>(true).FirstOrDefault();

                        if (siteContainsLanguageUrls)
                        {
                            if (page != null && !result.Contains(page) && page.SafeUrl.Contains(languageCode))
                                result.Add(page);
                        }
                        else
                        {
                            if (page != null && !result.Contains(page))
                                result.Add(page);
                        }
                    }

                    this._Results = result;
                }

                if ((this._Results == null) || (this._Results.Count() == 0))
                    return new List<WebPage>();
                return this._Results;
            }
        }
    }
}
