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
        /// <summary>
        /// Converts the <see cref="FormSubmission"/> into a <see cref="NameValueCollection"/>.
        /// </summary>
        /// <param name="submission">Specifies the submission to convert.</param>
        /// <returns>A <see cref="NameValueCollection"/> containing the same data as the 
        /// submission.</returns>
        public static NameValueCollection ToNameValueCollection(this FormSubmission submission)
        {
            if (submission == null)
                throw new ArgumentNullException("submission", "submission is null.");

            NameValueCollection result = new NameValueCollection(submission.Fields.Count);
            foreach (FormSubmissionField field in submission.Fields)
                result.Add(field.Name, field.Title);

            return result;
        }

        public static SyndicationItem ToSyndicationItem(this FormSubmission submission, string titleFieldFormat, string itemContentFormat, string fieldFormat)
        {
            if (submission == null)
	            throw new ArgumentNullException("submission", "submission is null.");
            if (String.IsNullOrEmpty(titleFieldFormat))
                throw new ArgumentException("titleFieldFormat is null or empty.", "titleFieldFormat");
            if (String.IsNullOrEmpty(itemContentFormat))
		        throw new ArgumentException("displayTemplate is null or empty.", "displayTemplate");
            if (String.IsNullOrEmpty(fieldFormat))
		        throw new ArgumentException("fieldFormat is null or empty.", "fieldFormat");

            NameValueCollection submissionItems = submission.ToNameValueCollection();
            
            string title = titleFieldFormat.ReplaceVariables(fieldFormat, submissionItems);
            string content = itemContentFormat.ReplaceVariables(fieldFormat, submissionItems);

            return new SyndicationItem(
                title,
                content, 
                submission.Page.SafeUrl.ToUri());
        }
    }
}
