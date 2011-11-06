<%@ Control Language="C#" AutoEventWireup="true" Inherits="HereSay.UI.Themed.Parts.NavigationMenuTemplate" %>
<%@ Import Namespace="HereSay.Parts" %>

<div class="menu">
    <ul>
    <% foreach (NavigationMapItem item in this.CurrentItem.NavigationMap.MapItems)
        {  
          %><li style="float:left"><a href="<%= item.DestinationUrl %>"><%= item.Title %></a></li><% 
        } %>
    </ul>
</div>