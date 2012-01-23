using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Rolcore.Web.UI;
using System;

namespace HereSay.Plugins.Forms
{

    [N2.PartDefinition(
        Title = "Expression Validator",
        Name = "FormExpressionValidator",
        IconUrl = "~/N2/Resources/icons/shape_square_error.png")]    
    public class FormExpressionValidator : FormValidator
    {
        #region Help Constants
        private const string ExpressionHelpTitle = 
            "Use a regular expression pattern that matches your preferred input format.";
        private const string ExpressionHelpText = 
            "<p>A regular expression (regex for short) is a special text string for describing a "
            +"search pattern. Here are some common patterns as an example (these can be used for "
            +"actual validation):</p><ol><li><strong>.+</strong> - matches a string that''s not "
            +"empty, so it will not allow a field to be blank.</li><li>"
            +@"<strong>\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*</strong> - matches a string "
            +"formatted like an email address.</li><li>"
            +@"<strong>^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$</strong> - "
            +"matches a date in mm/dd/yyyy format from between 01/01/1900 and 12/31/2099.</li><li>"
            +@"<strong>^\d{{5}}(-\d{{4}})?$</strong> - matches a Zip Code in the 00000 or "
            +"00000-0000 format.</li><li>"
            +@"<strong>^\d{{3}}[-/.]?\d{{3}}[-/.]?\d{{4}}$</strong> - matches a <em>United States"
            +"</em> Phone Number format: 123.456.7890, 123-456-7890, 1234567890.</li></ol><p>"
            +@"<a href=\http://www.regular-expressions.info\ target=\_blank\>"
            +"Regular-Expressions.Info</a> is a good place to start to learn more.</p>";
        #endregion Help Constants

        #region UI Properties

        [N2.Details.EditableTextBox("Expression", 20, Required = true, RequiredText = "*", 
            HelpTitle=ExpressionHelpTitle,
            HelpText = ExpressionHelpText)]
        public virtual string Expression
        {
            get { return GetDetail<string>("Expression", ".+"); } //".+" is a regular expression for "one-or-more of ANY character"
            set { SetDetail<string>("Expression", value); }
        }

        [N2.Details.EditableCheckBox("Reverse Match", 30, 
            HelpText = "Will negate the effect of the regular expression, if you DO NOT want to allow values that match the Expression.")]
        public virtual bool ReverseMatch
        {
            get { return GetDetail<bool>("ReverseMatch", false); }
            set { SetDetail<bool>("ReverseMatch", value); }
        }

        #endregion

        public override bool IsValid
        {
            get
            {
                if(string.IsNullOrEmpty(Title))
                    throw new InvalidOperationException("Cannot determine IsValid when no Field ID is specified.");
                if (string.IsNullOrEmpty(Expression))
                    throw new InvalidOperationException("Cannot determine IsValid when no Expression is specified.");

                string fieldValue = WebUtils.GetFormControlValueByFormControlId(
                    base.Title, 
                    FormUtils.NameValueCollectionFromRequest(HttpContext.Current.Request));

                if (fieldValue == null) fieldValue = "";
                bool doesValueMatch = Regex.IsMatch(fieldValue.Trim(), this.Expression);
                return this.ReverseMatch ? !doesValueMatch : doesValueMatch;
            }
        }
    }
}
