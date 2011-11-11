using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HereSay.Parts;
using Rolcore.Web;
using System.Web;
using N2.Collections;

namespace HereSay.Pages
{
    [N2.PageDefinition(
        Title = "Home Page",
        SortOrder = PageSorting.FrequentUse,
        InstallerVisibility = N2.Installation.InstallerHint.NeverRootPage 
                            | N2.Installation.InstallerHint.PreferredStartPage,
        IconUrl = "~/N2/Resources/icons/house.png"),
     N2.Integrity.RestrictParents(
        typeof(WebsiteRoot), 
        typeof(LanguageHomeRedirectPage))]
    public class HomePage : WebPage, N2.Web.ISitesSource
    {
        [N2.Details.EditableChildren(EditModeFields.Navigation, EditModeFields.Navigation, 100, ContainerName = EditModeTabs.Navigation, ZoneName=HereSayZones.NavigationMaps)]
        public IList<NavigationMap> NavigationMaps
        {
            get
            {
                return new ItemList<NavigationMap>(
                    this.Children,
                    new AccessFilter(),
                    new TypeFilter(typeof(NavigationMap)));
            }
        }

        [Details.EditableDropDownList(EditModeFields.Theme, 5, "AvailableThemes", ContainerName = EditModeTabs.LookAndFeelName)]
        public override string ThemeName
        {
            get { return this.GetDetail<string>("ThemeName", WebsiteRoot.Current.ThemeName); }
            set 
            {
                if (value == WebsiteRoot.Current.ThemeName)
                    this.SetDetail<string>("ThemeName", null); // Force default so that root changes will cascade
                else
                    this.SetDetail<string>("ThemeName", value); // Only store overrides
            }
        }

        #region ISitesSource Members
        [N2.Details.EditableTextBox("Hosts", 45, ContainerName = EditModeTabs.PageInformationName, HelpText="A host is the domain (e.g. www.yoursite.com) of your website. If a single website serves multiple domains, seperate each host with a comma; for example: www.site1.com,www.site2.com.")]
        public virtual string Hosts
        {
            //
            // Stored has "Host" rather than "Hosts" for compatibility with previous versions that
            // permitted only a single value.
            get 
            { 
                return GetDetail<string>(
                    "Host",  
                    (HttpContext.Current != null) 
                        ? HttpContext.Current.GetSiteBaseUrl().Authority
                        : null);
            }
            set { SetDetail("Host", value, string.Empty); }
        }

        public IEnumerable<N2.Web.Site> GetSites()
        {
            string[] hosts = this.Hosts.Split(',');
            foreach (string host in hosts)
                yield return new N2.Web.Site(Parent.ID, ID, host.Trim());
        }

        #endregion
    }
}
