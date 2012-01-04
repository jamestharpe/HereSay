<%@ Control Language="C#" AutoEventWireup="true" Inherits="HereSay.UI.Themed.Parts.SyndicatedContentTemplate" %>
<%  if (this.CurrentItem.Feed.Items.Any())
    {
        foreach (System.ServiceModel.Syndication.SyndicationItem feedItem in this.CurrentItem.Feed.Items)
        {%>
        <div>
            <h2><a href="<%: feedItem.Id %>"><%: feedItem.Title.Text%></a></h2>
            <span><%: feedItem.PublishDate.DateTime.ToShortDateString()%></span>
            <div><%= (feedItem.Summary != null) ? feedItem.Summary.Text : this.CurrentItem.GetContent(feedItem)%></div>
        </div>
        <%}
    }
    else
    {%>
        Nothing to display.
  <%} %>