
namespace HereSay
{
    public static class EditModeFields
    {
        public const string
            Navigation = "Navigation",
            Theme = "Theme";

        public static class Syndication
        {
            public const string
                ExcludeFromFeedName = "ExcludeFromFeed",
                ExcludeFromFeedTitle = "Exclude from Syndication Feeds.",
                ExcludeFromFeedHelpTitle = "",
                ExcludeFromFeedHelpBody = "This page will not be included in web feeds.",
                ExcludeFromSiteMapName = "ExcludeFromSiteMap",
                ExcludeFromSiteMapTitle = "Exclude from Sitemap";
            public const bool
                ExcludeFromFeedDefaultValue = false;

            public const string
                ContentPublishDateName = "OriginallyPublishedDate",
                ContentPublishDateTitle = "Published Date",
                ContentPublishDateHelpTitle = "",
                ContentPublishDateHelpBody = "";
            public const bool
                ContentPublishDateShowDateBox = true,
                ContentPublishDateShowTimeBox = true;
        }
    }
}
