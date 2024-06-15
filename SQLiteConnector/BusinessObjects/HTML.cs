using System;
using System.Collections.Generic;
using System.Text;

namespace SQLite.BusinessObjects
{
    internal static class HTML
    {
        public static string HtmlDecode(string html)
        {
            if (html == null)
                return string.Empty;
            string attribute = null; System.Text.StringBuilder result = new System.Text.StringBuilder();
            for (int i = 0; i < html.Length; i++)
            {
                char c = html[i];
                switch (c)
                {
                    case '<': attribute = ""; break;
                    case '>':
                        if (attribute == null)
                            result.Append('>');
                        else
                        {
                            if (attribute.StartsWith("br", System.StringComparison.CurrentCultureIgnoreCase)) result.Append('\n');
                            if (attribute.StartsWith("p ", System.StringComparison.CurrentCultureIgnoreCase) && result.Length > 0) result.Append('\n');
                        }
                        attribute = null; break;
                    case '&':
                        if (attribute == null)
                        {
                            if (html.Length >= i + 4 && html.Substring(i, 4) == "&gt;")
                            { result.Append('>'); i += 4 - 1; }
                            else if (html.Length >= i + 4 && html.Substring(i, 4) == "&lt;")
                            { result.Append('<'); i += 4 - 1; }
                            else if (html.Length >= i + 6 && html.Substring(i, 6) == "&nbsp;")
                            { result.Append(' '); i += 6 - 1; }
                        }
                        else
                            attribute += c;
                        break;

                    default:
                        if (attribute == null)
                        {
                            result.Append(c);
                        }
                        else
                        {
                            attribute += c;
                        }
                        break;
                }
            }

            return result.ToString();
        }
    }
}
