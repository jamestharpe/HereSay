<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationMenu.ascx.cs" Inherits="HereSay._hs.Themes.Boring.Parts.NavigationMenu" %>
<%@ Import Namespace="HereSay.Parts" %>

<div class="menu">
    <ul>
    <% foreach (NavigationMapItem item in this.CurrentItem.NavigationMap.MapItems)
        {  
          %><li style="float:left"><a href="<%= item.DestinationUrl %>"><%= item.Title %></a></li><% 
        } %>
    </ul>
</div>