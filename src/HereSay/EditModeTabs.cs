using System;

namespace HereSay
{
    public static class EditModeTabs
    {
        public const int SortIncrement = 100;

        public const string PageInformationName = "PageInformation";
        public const string PageInformationTitle = "Page Information";
        public const int PageInformationSortOrder = SortIncrement;

        public const string Content = "Content";
        public const int ContentSortOrder = PageInformationSortOrder + SortIncrement;

        public const string Navigation = "Navigation";
        public const int NavigationSortOrder = ContentSortOrder + SortIncrement;

        public const int SearchEngineOptimizationSortOrder = NavigationSortOrder + SortIncrement;

        public const string LookAndFeelName = "LookAndFeel";
        public const string LookAndFeelTitle = "Look & Feel";
        public const int LookAndFeelSortOrder = SyndicationSortOrder + SortIncrement;

        public const string Advanced = "Advanced";
        public const int AdvancedSortOrder = 1000;

        public const string Experiment = "Experiment";
        public const int SortOrder = MediaSortOrder - SortIncrement;
        
        public const string Media = "Media";
        public const int MediaSortOrder = AdvancedSortOrder - SortIncrement;

        public const string
            SyndicationName = "Syndication",
            SyndicationTitle = "Syndication";
        public const int
            SyndicationSortOrder = SearchEngineOptimizationSortOrder + SortIncrement;

        public const string
            FormValidationName = "FormValidation",
            FormValidationTitle = "Validation";
        public const int
            FormValidationSortOrder = ContentSortOrder + SortIncrement;

        public const string
            FormActionName = "FormAction",
            FormActionTitle = "Actions";
        public const int
            FormActionSortOrder = FormValidationSortOrder + SortIncrement;
        
    }
}
