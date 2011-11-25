using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using Rolcore;
using Rolcore.Web;
using System.Collections.Specialized;

namespace HereSay.Plugins.Forms
{
    public static class FormSubmissionExtensions
    {
        public static NameValueCollection ToNameValueCollection(this FormSubmission submission)
        {
            if (submission == null)
                throw new ArgumentNullException("submission", "submission is null.");

            NameValueCollection result = new NameValueCollection(submission.Fields.Count);
            foreach (FormSubmissionField field in submission.Fields)
                result.Add(field.Name, field.Title);

            return result;
        }

        public static SyndicationItem ToSyndicationItem(this FormSubmission submission, string titleFieldName, string displayTemplate, string fieldFormat)
        {
            if (submission == null)
	            throw new ArgumentNullException("submission", "submission is null.");
            if (String.IsNullOrEmpty(titleFieldName))
		        throw new ArgumentException("titleFieldName is null or empty.", "titleFieldName");
            if (String.IsNullOrEmpty(displayTemplate))
		        throw new ArgumentException("displayTemplate is null or empty.", "displayTemplate");
            if (String.IsNullOrEmpty(fieldFormat))
		        throw new ArgumentException("fieldFormat is null or empty.", "fieldFormat");

            FormSubmissionField titleField = submission.Fields
                .Where(field => 
                    field.Name == titleFieldName)
                .SingleOrDefault();
            string content = displayTemplate.ReplaceVariables(fieldFormat, submission.ToNameValueCollection());
            return new SyndicationItem(
                (titleField != null) ? titleField.Title : string.Empty,
                content, 
                submission.Page.SafeUrl.ToUri());
        }
    }
}
