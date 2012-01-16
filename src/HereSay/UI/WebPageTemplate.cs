using System;
using N2;
using N2.Collections;
using N2.Web.UI;
using HereSay.Engine;
using HereSay.Parts;
using HereSay.Pages;
using System.Diagnostics;

namespace HereSay.UI
{
    /// <summary>
    /// Base class for implementing a template for a webpage.
    /// </summary>
    /// <typeparam name="TPage">Specifies the type of page for which the instance is a template 
    /// for.</typeparam>
    public abstract class WebPageTemplate<TPage> : ContentPage<TPage>
        where TPage : N2.ContentItem
    {
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            N2.ContentItem currentPage = this.CurrentPage;
            WebPage webPage = currentPage as WebPage;
            this.Title = (webPage != null) 
                ? webPage.PageTitle
                : currentPage.Title;
        }
    }
}
