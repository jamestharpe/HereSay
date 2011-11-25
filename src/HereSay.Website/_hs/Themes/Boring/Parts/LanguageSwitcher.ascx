<%@ Control Language="C#" AutoEventWireup="true" Inherits="HereSay.UI.Themed.Parts.LanguageSwitcherTemplate" %>
<%@ Import Namespace="HereSay.Globalization" %>
<%@ Import Namespace="HereSay.Parts" %>
<%@ Import Namespace="HereSay" %>
<% 
foreach (ContentTranslation translation in this.CurrentItem.Translations){
    if(translation.Language.LanguageTitle != this.CurrentItem.CurrentLanguage.LanguageTitle)
    {
        string shortLanguageTitle = translation.Language.LanguageTitle.Substring(0, translation.Language.LanguageTitle.IndexOf(' '));
        %><img src="<%= N2.Web.Url.ToAbsolute(translation.Language.FlagUrl) %>" alt="<%= shortLanguageTitle %>" /><a class="language" href="<%= translation.Page.GetSafeUrl() %>"><%= shortLanguageTitle%></a>&nbsp;<%
    }
} %>