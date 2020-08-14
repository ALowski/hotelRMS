using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace System.Web.Mvc.Html
{
    public static partial class HtmlHelpers
    {
        /// <summary>
        /// This HTML Helper will add a Bootstrap stylized select.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="self"></param>
        /// <param name="expression"></param>
        /// <param name="values">The list of values in the select. In this Dictionary, Key is the value and Value is the label.</param>
        /// <param name="blank">If the select can show a blank option. Default: false.</param>
        /// <param name="events">Custom properties to be added to the tag. Separate them using the equals (=) sign.</param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapSelectFor<TModel, TValue>(this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression, Dictionary<string, string> values, [Optional, DefaultParameterValue(false)] bool blank, [Optional] params string[] events)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);

            // Creates the select tag.
            var select = new TagBuilder("select");

            // Inserts the blank option, if true.
            if (blank)
                select.InnerHtml = (new TagBuilder("option") { InnerHtml = "" }).ToString();

            // Creates the options tags for the select.
            foreach (var option in values)
            {
                var opt = new TagBuilder("option");

                opt.Attributes.Add("value", option.Key);
                opt.InnerHtml = option.Value;

                // Set the selected option if it's according to the Model.
                if (metadata.Model != null)
                    if (metadata.Model.ToString() == option.Key)
                        opt.Attributes.Add("selected", "");

                // Adds the option to the main select.
                select.InnerHtml += opt.ToString();
            }

            // Replaces the value if the Model is not null.
            select.Attributes.Add("value", metadata.Model == null ? "" : metadata.Model.ToString());

            // General properties.
            select.Attributes.Add("id", metadata.PropertyName);
            select.Attributes.Add("name", metadata.PropertyName);

            // Stylize with Bootstrap.
            select.AddCssClass("form-control");
            select.Attributes.Add("type", "text");

            // Adds the custom attributes.
            Helpers.AddAtrributes(select, events);

            // Adds the validation properties.
            Helpers.AddValidationProperties(self, expression, select);

            // Returns the input.
            return new MvcHtmlString(select.ToString());
        }

        /// <summary>
        /// This HTML Helper will add a Bootstrap stylized select.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="self"></param>
        /// <param name="expression"></param>
        /// <param name="values">The list of values and labels in the select, separated by comma.</param>
        /// <param name="blank">If the select can show a blank option. Default: false.</param>
        /// <param name="events">Custom properties to be added to the tag. Separate them using the equals (=) sign.</param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapSelectFor<TModel, TValue>(this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression, string values, [Optional, DefaultParameterValue(false)] bool blank, [Optional] params string[] events)
        {
            // Converts the value from string to Dictionary.
            var dict = new Dictionary<string, string>();

            // Split the string.
            var valuesArray = values.Replace(" ,", ",").Replace(", ", ",").Split(',');

            for (int i = 0; i < valuesArray.Length; i++)
            {
                // Adds the converted values to the dictionary.
                dict.Add(valuesArray[i], valuesArray[i + 1]);

                // Adds one more to the counter to jump one iteration.
                i++;
            }

            // Executes the default helper with the converted string to dict.
            return BootstrapSelectFor(self, expression, dict, blank, events);
        }
    }
}