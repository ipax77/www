
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace www.pwa.Client {
    public static class HtmlHelpers {
        static string arrowDown = "<span class=\"oi oi-arrow-bottom\"></span>";
        static string arrowUp = "<span class=\"oi oi-arrow-top\"></span>";
        static string arrowEmpty = "<span>&nbsp;</span>";

        public static string GetTitleMarkdown(PropertyInfo pi, string interest, bool order) {
            string Name = GetPropertyDisplayName(pi);
            string Desc = GetPropertyDescription(pi);
            string Arrow = GetArrow(pi, interest, order);
            return (!String.IsNullOrEmpty(Desc), !String.IsNullOrEmpty(Arrow)) switch {
                (true, false) => $"<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"{Desc}\">{Name}</span> ",
                (true, true) => $"<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"{Desc}\">{Name}</span> {Arrow}",
                (false, true) => $"{Name} {Arrow}",
                _ => $"{Name} "
            };
        }

        public static string GetPropertyDisplayName(PropertyInfo pi)
        {
            var dp = pi.GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().SingleOrDefault();
            return dp != null ? dp.Name : pi.Name;
        }

        public static string GetPropertyDescription(PropertyInfo pi) {
            var attrib = (DisplayAttribute)Attribute.GetCustomAttribute(pi, typeof(DisplayAttribute));
            return attrib == null ? "" : attrib.GetDescription();
        }

        public static string GetTooltip(PropertyInfo pi) {
            var description = GetPropertyDescription(pi);

            if (String.IsNullOrEmpty(description))
                description = GetPropertyDisplayName(pi);
            else
                return description;
            if (String.IsNullOrEmpty(description))
                description = pi.Name;
            return description;
        }

        

        public static int GetPropertyPrompt(PropertyInfo pi) {
            var attrib = (DisplayAttribute)Attribute.GetCustomAttribute(pi, typeof(DisplayAttribute));
            int prompt = 30;
            if (attrib == null)
                return prompt;
            int.TryParse(attrib.GetPrompt(), out prompt);
            return prompt;
        }

        static string GetArrow(PropertyInfo pi, string interest, bool order) {
            if (interest == pi.Name)
                if (order)
                    return arrowDown;
                else 
                    return arrowUp;
            else
                return arrowEmpty;
        } 
    }
}