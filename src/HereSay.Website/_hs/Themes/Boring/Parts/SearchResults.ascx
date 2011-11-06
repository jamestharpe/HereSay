<%@ Control Language="C#" AutoEventWireup="true" Inherits="HereSay.UI.Themed.Parts.SearchResultsTemplate" %>
<%@ Import Namespace="HereSay.Pages" %>

<% if (this.CurrentItem.Results.Any())
   {
       foreach (WebPage page in this.CurrentItem.Results)
       { 
           %><h3><a href="<%= page.SafeUrl %>"><%= page.PageTitle%></a></h3>
             <p><%= page.PageTitle%></p><% 
       }
   }
   else
   {
       %> No results for "<%= this.CurrentItem.SearchText %>" <%
   }%>