using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApp
{
    public class HtmlTemplateRenderer : IHtmlTemplateRenderer
    {
        public string RenderFromFile(string filePath, object dataModel)
        {
            throw new NotImplementedException();
        }

        public string RenderFromString(string htmlTemplate, object dataModel)
        {
            htmlTemplate = RenderForeach(htmlTemplate, dataModel);
            htmlTemplate = RenderIf(htmlTemplate, dataModel);
            htmlTemplate = RenderVars(htmlTemplate, dataModel, "");

            return htmlTemplate;
        }

        private string RenderVars(string htmlTemplate, object dataModel, string varName)
        {
            string pattern;
            if (varName == "")
            {
                pattern = @"\$\{([^}]+)\}";
            }
            else
            {
                pattern = @"\$\{" + Regex.Escape(varName) + @"(?:\.([^}]+))?\}";
            }

            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(htmlTemplate);

            foreach (Match match in matches)
            {
                string path = match.Groups[1].Value;
                object current;
                try
                {
                    current = path == "" ? dataModel : GetValue(path, dataModel);
                }
                catch (Exception)
                {
                    continue;
                }
                htmlTemplate = htmlTemplate.Replace(match.Value, current?.ToString() ?? "");
            }

            return htmlTemplate;
        }

        private string RenderIf(string htmlTemplate, object dataModel)
        {
            string ifPattern = @"\$if[ \t]*\(([^\r\n]+)\)[ \t]*\r?\n((?:(?!\$if|\$else|\$endif)[\s\S])*)(?:\$else[ \t]*\r?\n((?:(?!\$if|\$endif)[\s\S])*))?\$endif";

            Regex ifRegex = new Regex(ifPattern);

            while (true)
            {
                Match match = ifRegex.Match(htmlTemplate);
                if (!match.Success) break;

                string condition = match.Groups[1].Value;
                string blockIf = match.Groups[2].Value;
                string blockElse = match.Groups[3].Value;

                condition = RenderVars(condition, dataModel, "");

                string chosen = Checker(condition) ? blockIf : blockElse;

                htmlTemplate = htmlTemplate.Substring(0, match.Index) + chosen + htmlTemplate.Substring(match.Index + match.Length);
            }

            return htmlTemplate;
        }

        private string RenderForeach(string htmlTemplate, object dataModel, string parentVar = "")
        {
            string foreachPattern = @"\$foreach[ \t]*\((\w+)[ \t]+in[ \t]+([^\r\n]+)\)[ \t]*\r?\n((?:(?!\$foreach|\$endforeach)[\s\S]|\$foreach(?<d>)|\$endforeach(?<-d>))*)(?(d)(?!))\$endforeach";

            Regex regex = new Regex(foreachPattern);

            while (true)
            {
                Match match = regex.Match(htmlTemplate);
                if (!match.Success) break;

                string varName = match.Groups[1].Value;
                string collectionPath = match.Groups[2].Value.Trim();
                string body = match.Groups[3].Value;

                if (parentVar != "" && collectionPath.StartsWith(parentVar + ".", StringComparison.OrdinalIgnoreCase))
                {
                    collectionPath = collectionPath.Substring(parentVar.Length + 1);
                }

                object prop = GetValue(collectionPath, dataModel);
                IEnumerable items = prop as IEnumerable;
                if (items == null)
                {
                    throw new Exception();
                }

                StringBuilder sb = new StringBuilder();
                foreach (object item in items)
                {
                    string part = body;

                    foreach (Match inner in regex.Matches(body))
                    {
                        if (inner.Groups[1].Value == varName)
                        {
                            part = part.Replace(inner.Value, inner.Value.Replace("${" + varName, "${@" + varName));
                        }
                    }

                    part = RenderVars(part, item, varName);
                    part = part.Replace("${@" + varName, "${" + varName);
                    part = RenderForeach(part, item, varName);
                    sb.Append(part);
                }

                htmlTemplate = htmlTemplate.Substring(0, match.Index) + sb.ToString() + htmlTemplate.Substring(match.Index + match.Length);
            }

            return htmlTemplate;
        }

        private object GetValue(string path, object dataModel)
        {
            string[] parts = path.Trim().Split('.');
            object current = dataModel;

            for (int i = 0; i < parts.Length; i++)
            {
                if (current == null)
                {
                    throw new Exception();

                }


                parts[i] = char.ToUpper(parts[i][0]) + parts[i].ToLower().Substring(1);

                PropertyInfo prop = current.GetType().GetProperty(parts[i]);
                if (prop == null)
                {
                    throw new Exception();
                }

                current = prop.GetValue(current);
            }

            return current;
        }

        private bool Checker(string condition)
        {
            bool groupIsTrue = false;
            bool result;
            string[] orPaths = condition.Split(new string[] { "||" }, StringSplitOptions.None);
            foreach (string orPath in orPaths)
            {
                groupIsTrue = true;
                string[] andPaths = orPath.Split(new string[] { "&&" }, StringSplitOptions.None);
                foreach (string andPath in andPaths)
                {
                    Match m = Regex.Match(andPath.Trim(), @"^(.+?)\s*(==|!=|>=|<=|>|<)\s*(.+)$");
                    if (!m.Success)
                    {
                        result = bool.Parse(andPath);
                    }
                    else
                    {
                        string left = m.Groups[1].Value.Trim();
                        string op = m.Groups[2].Value;
                        string right = m.Groups[3].Value.Trim();

                        decimal a;
                        decimal b;
                        int c;
                        if (decimal.TryParse(left, out a) && decimal.TryParse(right, out b))
                        {
                            c = a.CompareTo(b);
                        }
                        else
                        {
                            c = string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
                        }
                        switch (op)
                        {
                            case "==":
                                result = (c == 0);
                                break;
                            case "!=":
                                result = (c != 0);
                                break;
                            case ">":
                                result = (c > 0);
                                break;
                            case "<":
                                result = (c < 0);
                                break;
                            case ">=":
                                result = (c >= 0);
                                break;
                            case "<=":
                                result = (c <= 0);
                                break;
                            default:
                                throw new Exception("Ты где этот знак нашел?" + op);
                        }
                    }
                    if (!result)
                    {
                        groupIsTrue = false;
                        break;
                    }
                }
                if (groupIsTrue) return true;
            }
            return false;
        }

        public string RenderToFile(string inputFilePath, string outputFilePath, object dataModel)
        {
            throw new NotImplementedException();
        }
    }
}