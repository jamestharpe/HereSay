using System;
using System.Linq;
using HereSay.Pages;

namespace HereSay.Parts
{
    [N2.PartDefinition(
        Title = "Navigation Menu",
        IconUrl = "~/N2/Resources/icons/tab.png"),
     N2.Details.WithEditableTitle(ContainerName=EditModeTabs.LookAndFeelName, SortOrder=0)]
    public class NavigationMenu : ThemedWebPart
    {
        protected NavigationMap _NavigationMap;
        protected string[] _AvailableMaps = null;

        public virtual string[] AvailableMaps
        {
            get
            {
                if (this._AvailableMaps == null)
                {
                    HomePage homePage = Find.FirstPublishedParent<HomePage>(this, true);

                    if(homePage == null)
                        throw new InvalidOperationException("A home page is required to manage navigation within your website.");

                    string[] result = homePage.NavigationMaps.Select(map => map.Title).ToArray();

                    if(result.Length == 0)
                        throw new InvalidOperationException("You have not configured any Navigation Maps on the Home Page at "+ homePage.SafeUrl +".");

                    this._AvailableMaps = result;
                }

                return this._AvailableMaps;
            }
        }

        [Details.EditableDropDownList("Map", 10, "AvailableMaps", ContainerName=EditModeTabs.LookAndFeelName)]
        public string MapTitle
        {
            get { return this.GetDetail<string>("MapTitle", WebsiteRoot.Current.ThemeName); }
            set { this.SetDetail<string>("MapTitle", value); }
        }

        public virtual NavigationMap NavigationMap
        {
            get
            {
                if (this._NavigationMap == null)
                {
                    HomePage homePage = Find.FirstPublishedParent<HomePage>(this, true);
                    this._NavigationMap = homePage.NavigationMaps.Where(map => map.Title == this.MapTitle).Single();
                }

                return this._NavigationMap;
            }
        }

        //TODO: Auto-detect the "current" NaviagionMapItem
    }
}
