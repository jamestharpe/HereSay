﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="HereSay._hs.Themes.Boring.Parts.SearchResults" %>
<%@ Import Namespace="HereSay.Pages" %>

<% 
foreach (WebPage page in this.CurrentItem.Results)
{ 
    %><h3><a href="<%= page.SafeUrl %>"><%= page.PageTitle %></a></h3>
      <p><%= page.PageTitle %></p><% 
} %>