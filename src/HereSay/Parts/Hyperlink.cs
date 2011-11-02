using System;
using Rolcore.Web;
using Rolcore;

namespace HereSay.Parts
{
    [N2.Details.WithEditableTitle("Link text", 0, Required = false)]
    public class Hyperlink : N2.ContentItem
    {
        private N2.ContentItem _DestinationWebPage;

        protected const string DestinationUrlItemKey = "DestinationUrl";

        protected string DefaultTitle
        {
            get
            {
                if (this.DestinationWebPage == null)
                    return null;

                return this.DestinationWebPage.Title;
            }
        }

        protected string QualifiedDestinationUrl
        {
            get
            {
                string baseUrl;
                N2.ContentItem startPage = Find.StartPage;
                if (startPage != null)
                    baseUrl = startPage.GetSafeUrl();
                else
                    // TODO: Handle null context
                    baseUrl = N2.Context.Current.Host.CurrentSite.Authority;

                System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(baseUrl), "Base URL not set for website.");

                Uri result = new Uri(new Uri(baseUrl), this.DestinationUrl);

                return result.ToString();
            }
        }

        protected N2.ContentItem DestinationWebPage
        {
            get
            {
                if (this._DestinationWebPage == null && this.DestinationUrl != null)
                    this._DestinationWebPage = Find.ByUrl(this.QualifiedDestinationUrl);
                return this._DestinationWebPage;
            }
            set
            {
                //
                // Allows sub-classes to short-cut the re-evaluation of the DestinationUrl, which
                // can be a lengthy operation.

                this._DestinationWebPage = value;
                if (value != null)
                    SetDetail<string>(DestinationUrlItemKey, value.GetSafeUrl().ToUri().AbsolutePath);
            }
        }

        [N2.Details.EditableCheckBox("Add rel=\"nofollow\"", 50)]
        public bool RelNoFollow
        {
            get { return GetDetail<bool>("RelNoFollow", false); }
            set { SetDetail<bool>("RelNoFollow", value); }
        }

        [N2.Details.EditableTextBox("Target", 40)]
        public string Target
        {
            get { return GetDetail<string>("Target", String.Empty); }
            set { SetDetail<string>("Target", value, String.Empty); }
        }

        [N2.Details.EditableUrl("Page / URL", 1, Required = true, RequiredText = "*")]
        public string DestinationUrl
        {
            get { return GetDetail<string>(DestinationUrlItemKey, null); }
            set
            {
                SetDetail<string>(DestinationUrlItemKey, value);
                
                // Force consistent hrefs
                this._DestinationWebPage = null; // Causes re-eval
                if (this.DestinationWebPage != null)
                {
                    Uri destinationUrl = new Uri(this._DestinationWebPage.GetSafeUrl());
                    if (destinationUrl.ToString() != this.DestinationUrl)
                        SetDetail<string>(DestinationUrlItemKey, destinationUrl.AbsolutePath);
                }
            }
        }

        public override string Title
        {
            get { return string.IsNullOrWhiteSpace(base.Title) ? this.DefaultTitle : base.Title; }
            set
            {
                if ((value == this.DefaultTitle) || (string.IsNullOrWhiteSpace(value)))
                    base.Title = null; // Force default
                else
                    base.Title = value;
            }
        }

        public bool DestinationIsCurrentPage
        {
            get
            {
                if (this.DestinationWebPage == null)
                    return false;

                return this.DestinationWebPage.ID == N2.Context.CurrentPage.ID;
            }
        }
    }
}
