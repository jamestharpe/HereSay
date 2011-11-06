<%@ Control Language="C#" AutoEventWireup="true" Inherits="HereSay.UI.Themed.Parts.LanguageSwitcherTemplate" %>
<%@ Import Namespace="HereSay.Globalization" %>
<%@ Import Namespace="HereSay.Parts" %>
<%@ Import Namespace="HereSay" %>

<% foreach (ContentTranslation translation in this.CurrentItem.Translations){
    if (translation.Language.LanguageTitle != LanguageSwitcher.CurrentLanguage.LanguageTitle)
    { 
        %><img src="<%= translation.Language.FlagUrl %>" alt="<%= translation.Language.LanguageTitle %>" /><a class="language" href="<%= translation.Page.GetSafeUrl() %>"><%= translation.Language.LanguageTitle %></a>&nbsp;<%
    }
} %>