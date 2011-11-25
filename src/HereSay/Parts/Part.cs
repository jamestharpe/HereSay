
using HereSay.Pages;
namespace HereSay.Parts
{
    /// <summary>
    /// Base class for Parts.
    /// </summary>
    public abstract class Part : N2.ContentItem //TODO: ContentPart
    {
        public override bool IsPage { get { return false; } }

        /// <summary>
        /// Gets the <see cref="WebPage"/> the current item is contained within.
        /// </summary>
        public WebPage Page
        {
            get { return Find.FirstPublishedParent<WebPage>(this, true); }
        }
    }
}
