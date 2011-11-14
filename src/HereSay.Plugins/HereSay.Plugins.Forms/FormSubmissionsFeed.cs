using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HereSay.Pages;
using System.ServiceModel.Syndication;

namespace HereSay.Plugins.Forms
{
    public class FormSubmissionsFeed : FeedPageBase
    {
        public override SyndicationFeed Feed
        {
            get { throw new NotImplementedException(); }
        }
    }
}
