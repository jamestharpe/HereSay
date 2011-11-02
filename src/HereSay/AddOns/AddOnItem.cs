
namespace HereSay.AddOns
{
    /// <summary>
    /// Base class for AddOns.
    /// <see cref="https://rollins.fogbugz.com/default.asp?W629"/>
    /// </summary>
    public abstract class AddOnItem : N2.ContentItem //TODO: ContentPart
    {
        public override bool IsPage
        {
            get { return false; }
        }
    }
}
