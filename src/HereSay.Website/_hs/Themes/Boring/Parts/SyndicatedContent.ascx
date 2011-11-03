<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SyndicatedContent.ascx.cs" Inherits="HereSay.Website._hs.Themes.Boring.Parts.SyndicatedContent" %>
<%
    if (this.CurrentItem.Feed.Items.Any())
    {
        foreach (System.ServiceModel.Syndication.SyndicationItem feedItem in this.CurrentItem.Feed.Items)
        {%>
        <div>
            <h2><a href="<%: feedItem.Id %>"><%: feedItem.Title.Text%></a></h2>
            <span><%: feedItem.PublishDate.DateTime.ToShortDateString()%></span>
            <div><%= feedItem.Summary.Text%></div>
        </div>
        <%}
    }
    else
    {%>
        Nothing to display.
    <%}   
%>