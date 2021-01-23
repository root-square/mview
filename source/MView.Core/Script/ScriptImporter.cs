using MView.Core.Extension;
using MView.Core.Script.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MView.Core.Script
{
    /// <summary>
    /// Provides methods to import scripts.
    /// </summary>
    public class ScriptImporter
    {
        // TODO : 예외처리하기.
        private List<JPathCollection> _jPathCollections = new List<JPathCollection>();

        /// <summary>
        /// Create a new ScriptImporter instance.
        /// </summary>
        /// <param name="jPathCollections"></param>
        public ScriptImporter(List<JPathCollection> jPathCollections)
        {
            _jPathCollections = jPathCollections;
        }

        /// <summary>
        /// Imports the specified data to a JSON file.
        /// </summary>
        /// <param name="filePath">Data file to import.</param>
        /// <param name="jsonPath">Original JSON file.</param>
        /// <param name="savePath">The file in which the modified data will be stored.</param>
        /// <param name="jPaths">List of JPath data to import.</param>
        private void Import(string filePath, string jsonPath, string savePath)
        {
            // Check variables.
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Data file not found.");
            }

            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            }

            // Get JPaths.
            List<string> jPaths = _jPathCollections.GetJPaths(filePath);

            // Initialize variables.
            string result = string.Empty;

            string[] transArray = FileManager.ReadTextFile(filePath, Encoding.UTF8).SplitByString("\r\n");
            int transIndex = 0;

            // Load and Parse Original JSON File.
            string source = FileManager.ReadTextFile(jsonPath, Encoding.UTF8);
            JToken json = JToken.Parse(source);

            JToken[] jTokenArray = new JToken[6];
            JArray jArray = JArray.Parse("[]");

            if (json.Type == JTokenType.Array) // Part of JArray parsing.
            {
                foreach (JToken field in json)
                {
                    if (field.Type != JTokenType.Null)
                    {
                        foreach (string jPath in jPaths)
                        {
                            foreach (JToken selectedToken in field.SelectTokens(jPath))
                            {
                                if (selectedToken.Type == JTokenType.Array)
                                {
                                    jTokenArray[0] = selectedToken.SplitJToken(ref result, ref transArray, ref transIndex, true);
                                    selectedToken.Replace(jTokenArray[0]);
                                }
                                else
                                {
                                    int parsingResult = 0;
                                    JToken temp;

                                    if (int.TryParse(transArray[transIndex], out parsingResult))
                                    {
                                        temp = new JValue(parsingResult);
                                    }
                                    else
                                    {
                                        temp = new JValue(transArray[transIndex].Replace("\\\\n", "\\\\AC").Replace("\\n", "\n").Replace("\\\\AC", "\\n").Replace("\"", "”"));
                                    }

                                    selectedToken.Replace(temp);
                                    transIndex++;
                                }
                            }
                        }
                    }
                    jArray.Add(field);
                }

                FileManager.WriteTextFile(savePath, jArray.ToString().Replace("\"$null!\"", "null"), Encoding.UTF8);
            }
            else // Part of JObject parsing.
            {
                JObject jObject = JObject.Parse("{}");

                foreach (string jPath in jPaths)
                {
                    foreach (JToken selectedToken in json.SelectTokens(jPath))
                    {
                        if (selectedToken.Type == JTokenType.Array)
                        {
                            jTokenArray[0] = selectedToken.SplitJToken(ref result, ref transArray, ref transIndex, true);
                            selectedToken.Replace(jTokenArray[0]);
                        }
                        else
                        {
                            int parsingResult = 0;
                            JToken temp;

                            if (int.TryParse(transArray[transIndex], out parsingResult))
                            {
                                temp = new JValue(parsingResult);
                            }
                            else
                            {
                                temp = new JValue(transArray[transIndex].Replace("\\\\n", "\\\\AC").Replace("\\n", "\n").Replace("\\\\AC", "\\n").Replace("\"", "”"));
                            }

                            selectedToken.Replace(temp);
                            transIndex++;
                        }
                    }
                    jObject = (JObject)json;
                }


                FileManager.WriteTextFile(savePath, jObject.ToString().Replace("\"$null!\"", "null"), Encoding.UTF8);
            }
        }
    }
}
