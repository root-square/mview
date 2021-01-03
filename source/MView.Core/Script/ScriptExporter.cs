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
    /// Provides methods to export scripts.
    /// </summary>
    public class ScriptExporter
    {
        private List<JPathCollection> _jPathCollections = new List<JPathCollection>();

        /// <summary>
        /// Create a new ScriptExporter instance.
        /// </summary>
        /// <param name="jPathCollections"></param>
        public ScriptExporter(List<JPathCollection> jPathCollections)
        {
            _jPathCollections = jPathCollections;
        }

        /// <summary>
        /// Exports the specified data from a JSON file.
        /// </summary>
        /// <param name="filePath">The JSON file from which to extract data.</param>
        /// <param name="savePath">The file in which the extracted data will be stored.</param>
        public void Export(string filePath, string savePath)
        {
            try
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

                string[] transArray = new string[] { };
                int transIndex = 0;

                // Load and Parse JSON File.
                string source = FileManager.ReadTextFile(filePath, Encoding.UTF8);
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
                                        jTokenArray[0] = selectedToken.SplitJToken(ref result, ref transArray, ref transIndex, false);
                                    }
                                    else
                                    {
                                        result += selectedToken.NormalizeJToken();
                                    }
                                }
                            }
                        }
                    }
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
                                jTokenArray[0] = selectedToken.SplitJToken(ref result, ref transArray, ref transIndex, false);
                            }
                            else
                            {
                                result += selectedToken.NormalizeJToken();
                            }
                        }
                        jObject = (JObject)json;
                    }
                }

                FileManager.WriteTextFile(savePath, result, Encoding.UTF8);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
