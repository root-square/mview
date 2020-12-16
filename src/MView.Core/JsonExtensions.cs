using MView.Core.Script;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace MView.Core
{
    internal static class JsonExtensions
    {
        /// <summary>
        /// If the entered JToken is in Array type, it is split and returned.
        /// </summary>
        /// <param name="jToken">JToken to split.</param>
        /// <param name="text">A string variable where the result will be stored.</param>
        /// <param name="transArray">An array required for reverse operation.</param>
        /// <param name="transIndex">Current index of <c>transArray</c>.</param>
        /// <param name="isReverse">Whether reverse operation is in progress.</param>
        /// <returns>Split JArray</returns>
        internal static JToken SplitJToken(this JToken jToken, ref string text, ref string[] transArray, ref int transIndex, bool isReverse = false)
        {
            JToken result;
            JArray jArray = JArray.Parse("[]");

            if (jToken.Type == JTokenType.Array)
            {
                foreach (JToken field in jToken)
                {
                    if (field.Type == JTokenType.Array)
                    {
                        jArray.Add(SplitJToken(field, ref text, ref transArray, ref transIndex, isReverse)); // Recurs to SplitJToken so as to split JArray.
                    }
                    else if (isReverse)
                    {
                        JToken temp;
                        int parsingResult = 0;

                        if (int.TryParse(transArray[transIndex], out parsingResult))
                        {
                            temp = new JValue(parsingResult);
                        }
                        else
                        {
                            temp = new JValue(transArray[transIndex].Replace("\\\\n", "\\\\AC").Replace("\\n", "\n").Replace("\\\\AC", "\\n").Replace("\"", "”"));
                        }

                        transIndex++;

                        jArray.Add(temp);
                    }
                    else
                    {
                        text += NormalizeJToken(field);
                    }
                }
                result = jArray;
            }
            else if (isReverse)
            {
                JToken temp;
                int parsingResult = 0;

                if (int.TryParse(transArray[transIndex], out parsingResult))
                {
                    temp = new JValue(parsingResult);
                }
                else
                {
                    temp = new JValue(transArray[transIndex].Replace("\\\\n", "\\\\AC").Replace("\\n", "\n").Replace("\\\\AC", "\\n").Replace("\"", "”"));
                }

                transIndex++;
                result = temp;
            }
            else
            {
                text += NormalizeJToken(jToken);
                result = jToken;
            }

            return result;
        }

        /// <summary>
        /// Normalize JToken with string.
        /// </summary>
        /// <param name="jToken"></param>
        /// <returns>Normalized JToken</returns>
        internal static string NormalizeJToken(this JToken jToken)
        {
            string result = string.Empty;

            if (jToken.Type == JTokenType.Array)
            {
                foreach (JToken field in jToken)
                {
                    if (field.Type == JTokenType.Array)
                    {
                        string temp = NormalizeJToken(field);

                        result += temp;
                    }
                }
            }
            else
            {
                if (jToken.Type == JTokenType.Null)
                {
                    result += "$null!\r\n";
                }
                else
                {
                    result += jToken.ToString() + "\r\n";
                }
            }

            result = result.Replace(@"\n", @"\\AC");

            return result;
        }

        internal static List<string> GetJPaths(this List<JPathCollection> collections, string fileName)
        {
            foreach (JPathCollection collection in collections)
            {
                if (Path.GetFileNameWithoutExtension(fileName).Contains(collection.Header))
                {
                    return collection.Paths;
                }
            }

            return null;
        }
    }
}