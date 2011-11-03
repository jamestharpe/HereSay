
namespace HereSay.Parts
{
    /// <summary>
    /// Base class for Parts.
    /// </summary>
    public abstract class Part : N2.ContentItem //TODO: ContentPart
    {
        public override bool IsPage { get { return false; } }
    }
}
