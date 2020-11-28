using MView.Extensions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MView.Utilities
{
    public class TranslationHelper
    {
        private readonly List<string> _actorsPaths = new List<string>();
        private readonly List<string> _armorsPaths = new List<string>();
        private readonly List<string> _classesPaths = new List<string>();
        private readonly List<string> _commonPaths = new List<string>();
        private readonly List<string> _enemiesPaths = new List<string>();
        private readonly List<string> _itemsPaths = new List<string>();
        private readonly List<string> _mapsPaths = new List<string>();
        private readonly List<string> _systemPaths = new List<string>();
        private readonly List<string> _statesPaths = new List<string>();
        private readonly List<string> _skillsPaths = new List<string>();
        private readonly List<string> _troopsPaths = new List<string>();
        private readonly List<string> _weaponsPaths = new List<string>();
        private readonly List<string> _jsPaths = new List<string>();

        public TranslationHelper()
        {
            // Actors paths initialize.
            _actorsPaths.Add("name");
            _actorsPaths.Add("nickname");
            _actorsPaths.Add("profile");

            // Armors paths initialize.
            _armorsPaths.Add("description");
            _armorsPaths.Add("name");

            // Classes paths initialize.
            _classesPaths.Add("name");

            // Common paths initialize.
            _commonPaths.Add("$..list[?(@.code == 102)].parameters");
            _commonPaths.Add("$..list[?(@.code == 401)].parameters");

            // Enimes paths initialize.
            _enemiesPaths.Add("name");

            // Items paths initialize.
            _itemsPaths.Add("description");
            _itemsPaths.Add("name");

            // Maps paths initialize.
            _mapsPaths.Add("displayName");
            _mapsPaths.Add("$..list[?(@.code == 102)].parameters");
            _mapsPaths.Add("$..list[?(@.code == 401)].parameters");

            // System paths initialize.
            _systemPaths.Add("armorTypes");
            _systemPaths.Add("elements");
            _systemPaths.Add("equipTypes");
            _systemPaths.Add("gameTitle");
            _systemPaths.Add("skillTypes");
            _systemPaths.Add("terms.basic");
            _systemPaths.Add("terms.commands");
            _systemPaths.Add("terms.params");
            _systemPaths.Add("terms.messages.actionFailure");
            _systemPaths.Add("terms.messages.actorDamage");
            _systemPaths.Add("terms.messages.actorDrain");
            _systemPaths.Add("terms.messages.actorGain");
            _systemPaths.Add("terms.messages.actorLoss");
            _systemPaths.Add("terms.messages.actorNoDamage");
            _systemPaths.Add("terms.messages.actorNoHit");
            _systemPaths.Add("terms.messages.actorRecovery");
            _systemPaths.Add("terms.messages.alwaysDash");
            _systemPaths.Add("terms.messages.bgmVolume");
            _systemPaths.Add("terms.messages.bgsVolume");
            _systemPaths.Add("terms.messages.buffAdd");
            _systemPaths.Add("terms.messages.buffRemove");
            _systemPaths.Add("terms.messages.commandRemember");
            _systemPaths.Add("terms.messages.counterAttack");
            _systemPaths.Add("terms.messages.criticalToActor");
            _systemPaths.Add("terms.messages.criticalToEnemy");
            _systemPaths.Add("terms.messages.debuffAdd");
            _systemPaths.Add("terms.messages.defeat");
            _systemPaths.Add("terms.messages.emerge");
            _systemPaths.Add("terms.messages.enemyDamage");
            _systemPaths.Add("terms.messages.enemyDrain");
            _systemPaths.Add("terms.messages.enemyGain");
            _systemPaths.Add("terms.messages.enemyLoss");
            _systemPaths.Add("terms.messages.enemyNoDamage");
            _systemPaths.Add("terms.messages.enemyNoHit");
            _systemPaths.Add("terms.messages.enemyRecovery");
            _systemPaths.Add("terms.messages.escapeFailure");
            _systemPaths.Add("terms.messages.escapeStart");
            _systemPaths.Add("terms.messages.evasion");
            _systemPaths.Add("terms.messages.expNext");
            _systemPaths.Add("terms.messages.expTotal");
            _systemPaths.Add("terms.messages.file");
            _systemPaths.Add("terms.messages.levelUp");
            _systemPaths.Add("terms.messages.loadMessage");
            _systemPaths.Add("terms.messages.magicEvasion");
            _systemPaths.Add("terms.messages.magicReflection");
            _systemPaths.Add("terms.messages.meVolume");
            _systemPaths.Add("terms.messages.obtainExp");
            _systemPaths.Add("terms.messages.obtainGold");
            _systemPaths.Add("terms.messages.obtainItem");
            _systemPaths.Add("terms.messages.obtainSkill");
            _systemPaths.Add("terms.messages.partyName");
            _systemPaths.Add("terms.messages.possession");
            _systemPaths.Add("terms.messages.preemptive");
            _systemPaths.Add("terms.messages.saveMessage");
            _systemPaths.Add("terms.messages.seVolume");
            _systemPaths.Add("terms.messages.substitute");
            _systemPaths.Add("terms.messages.surprise");
            _systemPaths.Add("terms.messages.useItem");
            _systemPaths.Add("terms.messages.victory");
            _systemPaths.Add("weaponTypes");

            // States paths initialize.
            _statesPaths.Add("message1");
            _statesPaths.Add("message2");
            _statesPaths.Add("message3");
            _statesPaths.Add("message4");
            _statesPaths.Add("name");

            // Skills paths initialize.
            _skillsPaths.Add("message1");
            _skillsPaths.Add("message2");
            _skillsPaths.Add("name");
            _skillsPaths.Add("description");

            // Troops paths initialize.
            _troopsPaths.Add("$..list[?(@.code == 401)].parameters");

            // Weapons paths initialize.
            _weaponsPaths.Add("description");
            _weaponsPaths.Add("name");

            // Js paths initialize.
            _jsPaths.Add("parameters");
        }

        /*
        public void plugjs()
        {
            outtext = "";
            curruntfile = "plugins";
            if (isUnpack)
                transary = DoSplit(File.ReadAllText(extractedpath + "\\" + curruntfile + ".txt", Encoding.UTF8), "\r\n");
            if (isUnpack)
                tranproc = 0;
            if (File.Exists(pluginjspath))
            {
                JToken jtoken1 = JToken.Parse(DoSplit(DoSplit(File.ReadAllText(pluginjspath), "var $plugins =")[1], ";")[0]);
                bool flag = false;
                foreach (JToken jtoken2 in (IEnumerable<JToken>)jtoken1)
                {
                    foreach (JProperty jproperty in (IEnumerable<JToken>)jtoken2.SelectToken(PJs[0]))
                    {
                        if (isUnpack)
                        {
                            int result = 0;
                            if (!int.TryParse(transary[tranproc], out result))
                            {
                                if (jproperty.Value.ToString() != "true" && jproperty.Value.ToString() != "false")
                                {
                                    if (jproperty.Value.Type.ToString() != "Array")
                                    {
                                        try
                                        {
                                            foreach (JToken jtoken3 in JArray.Parse("[" + (object)jproperty.Value + "]"))
                                            {
                                                if (jtoken3.Type == JTokenType.Integer)
                                                    flag = true;
                                                if (!flag)
                                                {
                                                    JToken jtoken4 = (JToken)new JValue(transary[tranproc].Replace("\\\\n", "\\\\AC").Replace("\\n", "\n").Replace("\\\\AC", "\\n").Replace("\"", "”"));
                                                    ++tranproc;
                                                    jproperty.Replace(jtoken4);
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            JToken jtoken3 = (JToken)new JValue(transary[tranproc].Replace("\\\\n", "\\\\AC").Replace("\\n", "\n").Replace("\\\\AC", "\\n").Replace("\"", "”"));
                                            jproperty.Value = jtoken3;
                                            ++tranproc;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                JToken jtoken5 = (JToken)new JValue((long)result);
                            }
                        }
                        else
                        {
                            int result = 0;
                            if (!int.TryParse(jproperty.Value.ToString(), out result))
                            {
                                if (jproperty.Value.ToString() != "true" && jproperty.Value.ToString() != "false")
                                {
                                    if (jproperty.Value.Type.ToString() != "Array")
                                    {
                                        try
                                        {
                                            foreach (JToken jtoken3 in JArray.Parse("[" + (object)jproperty.Value + "]"))
                                            {
                                                if (jtoken3.Type == JTokenType.Integer)
                                                    flag = true;
                                                if (!flag)
                                                    outtext = outtext + optstr((object)jproperty.Value) + "\r\n";
                                            }
                                        }
                                        catch
                                        {
                                            outtext = outtext + optstr((object)jproperty.Value) + "\r\n";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                JValue jvalue = new JValue((long)result);
                            }
                        }
                    }
                }
                if (isUnpack)
                    outtext = "var $plugins =\n" + jtoken1.ToString() + ";";
                if (!isUnpack)
                    File.WriteAllText(extractedpath + "\\" + curruntfile + ".txt", outtext, Encoding.UTF8);
                if (isUnpack)
                    File.WriteAllText(completedpath + "\\" + curruntfile + ".js", outtext, Encoding.UTF8);
                msgbox((object)"종료");
            }
            else
                msgbox((object)"plugins.js 파일이 해당경로에 존제하지 않습니다.");
        }*/

        /// <summary>
        /// Exports the specified data from a JSON file.
        /// </summary>
        /// <param name="sourceDirectory">Directory that contains the data to export.</param>
        /// <param name="saveDirectory">Directory in which extracted files will be stored.</param>
        /// <param name="backupDirectory">Directory where the backup files will be stored.</param>
        /// /// <param name="createBackup">Specifies whether to create backup files.</param>
        public void ExportDatas(string sourceDirectory, string saveDirectory, string backupDirectory = null, bool createBackup = false)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException("Source data directory not found.");
            }

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            if (createBackup)
            {
                if (string.IsNullOrEmpty(backupDirectory))
                {
                    throw new FileNotFoundException("The file path cannot be empty.");
                }

                if (!Directory.Exists(backupDirectory))
                {
                    Directory.CreateDirectory(backupDirectory);
                }
            }

            foreach (string file in Directory.GetFiles(sourceDirectory))
            {
                if (Path.GetExtension(file).ToLower() != ".json")
                {
                    throw new FileLoadException("Only JSON format is supported.");
                }

                if (createBackup)
                {
                    string backupPath = Path.Combine(backupDirectory, Path.GetFileName(file));
                    File.Copy(file, backupPath);
                }

                string fileName = Path.GetFileNameWithoutExtension(file);
                string savePath = Path.Combine(saveDirectory, Path.GetFileNameWithoutExtension(file) + ".txt");

                switch (fileName.ToLower())
                {
                    case "actors":
                        Export(file, savePath, _actorsPaths);
                        continue;
                    case "armors":
                        Export(file, savePath, _armorsPaths);
                        continue;
                    case "classes":
                        Export(file, savePath, _classesPaths);
                        continue;
                    case "commonevents":
                        Export(file, savePath, _commonPaths);
                        continue;
                    case "enemies":
                        Export(file, savePath, _enemiesPaths);
                        continue;
                    case "items":
                        Export(file, savePath, _itemsPaths);
                        continue;
                    case "mapinfos":
                        continue;
                    case "skills":
                        Export(file, savePath, _skillsPaths);
                        continue;
                    case "states":
                        Export(file, savePath, _statesPaths);
                        continue;
                    case "system":
                        Export(file, savePath, _systemPaths);
                        continue;
                    case "troops":
                        Export(file, savePath, _troopsPaths);
                        continue;
                    case "weapons":
                        Export(file, savePath, _weaponsPaths);
                        continue;
                    default:
                        if (((IEnumerable<string>)fileName.ToLower().SplitByString("map")).Count() > 1)
                        {
                            Export(file, savePath, _mapsPaths);
                            continue;
                        }
                        continue;
                }
            }
        }

        /// <summary>
        /// Imports the specified data to a JSON file.
        /// </summary>
        /// <param name="sourceDirectory">Directory that contains the data to import.</param>
        /// <param name="jsonDirectory">Directory where the original JSON files reside.</param>
        /// <param name="saveDirectory">Directory in which modified files will be stored.</param>
        /// <param name="backupDirectory">Directory where the backup files will be stored.</param>
        /// <param name="createBackup">Specifies whether to create backup files.</param>
        public void ImportDatas(string sourceDirectory, string jsonDirectory, string saveDirectory, string backupDirectory = null, bool createBackup = false)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException("Source data directory not found.");
            }

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            if (createBackup)
            {
                if (string.IsNullOrEmpty(backupDirectory))
                {
                    throw new FileNotFoundException("The file path cannot be empty.");
                }

                if (!Directory.Exists(backupDirectory))
                {
                    Directory.CreateDirectory(backupDirectory);
                }
            }

            foreach (string file in Directory.GetFiles(sourceDirectory))
            {
                if (Path.GetExtension(file).ToLower() != ".txt")
                {
                    throw new FileLoadException("Only TXT format is supported.");
                }

                if (createBackup)
                {
                    string backupPath = Path.Combine(backupDirectory, Path.GetFileName(file));
                    File.Copy(file, backupPath);
                }

                string fileName = Path.GetFileNameWithoutExtension(file);
                string jsonPath = Path.Combine(jsonDirectory, Path.GetFileNameWithoutExtension(file) + ".json");
                string savePath = Path.Combine(saveDirectory, Path.GetFileNameWithoutExtension(file) + ".json");

                switch (fileName.ToLower())
                {
                    case "actors":
                        Import(file, jsonPath, savePath, _actorsPaths);
                        continue;
                    case "armors":
                        Import(file, jsonPath, savePath, _armorsPaths);
                        continue;
                    case "classes":
                        Import(file, jsonPath, savePath, _classesPaths);
                        continue;
                    case "commonevents":
                        Import(file, jsonPath, savePath, _commonPaths);
                        continue;
                    case "enemies":
                        Import(file, jsonPath, savePath, _enemiesPaths);
                        continue;
                    case "items":
                        Import(file, jsonPath, savePath, _itemsPaths);
                        continue;
                    case "mapinfos":
                        continue;
                    case "skills":
                        Import(file, jsonPath, savePath, _skillsPaths);
                        continue;
                    case "states":
                        Import(file, jsonPath, savePath, _statesPaths);
                        continue;
                    case "system":
                        Import(file, jsonPath, savePath, _systemPaths);
                        continue;
                    case "troops":
                        Import(file, jsonPath, savePath, _troopsPaths);
                        continue;
                    case "weapons":
                        Import(file, jsonPath, savePath, _weaponsPaths);
                        continue;
                    default:
                        if (((IEnumerable<string>)fileName.ToLower().SplitByString("map")).Count() > 1)
                        {
                            Import(file, jsonPath, savePath, _mapsPaths);
                            continue;
                        }
                        continue;
                }
            }
        }

        /// <summary>
        /// Exports the specified data from a JSON file.
        /// </summary>
        /// <param name="filePath">The JSON file from which to extract data.</param>
        /// <param name="savePath">The file in which the extracted data will be stored.</param>
        /// <param name="jPaths">List of JPath data to extract.</param>
        private void Export(string filePath, string savePath, List<string> jPaths)
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
                                    jTokenArray[0] = SplitJToken(selectedToken, ref result, ref transArray, ref transIndex, false);
                                }
                                else
                                {
                                    result += UnpackJToken(selectedToken);
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
                            jTokenArray[0] = SplitJToken(selectedToken, ref result, ref transArray, ref transIndex, false);
                        }
                        else
                        {
                            result += UnpackJToken(selectedToken);
                        }
                    }
                    jObject = (JObject)json;
                }
            }

            FileManager.WriteTextFile(savePath, result, Encoding.UTF8);
        }

        /// <summary>
        /// Imports the specified data to a JSON file.
        /// </summary>
        /// <param name="filePath">Data file to import.</param>
        /// <param name="jsonPath">Original JSON file.</param>
        /// <param name="savePath">The file in which the modified data will be stored.</param>
        /// <param name="jPaths">List of JPath data to import.</param>
        private void Import(string filePath, string jsonPath, string savePath, List<string> jPaths)
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
                                    jTokenArray[0] = SplitJToken(selectedToken, ref result, ref transArray, ref transIndex, true);
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
                FileManager.WriteTextFile(savePath, jArray.ToString(), Encoding.UTF8);
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
                            jTokenArray[0] = SplitJToken(selectedToken, ref result, ref transArray, ref transIndex, true);
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
                FileManager.WriteTextFile(savePath, jObject.ToString(), Encoding.UTF8);
            }
        }

        /// <summary>
        /// If the entered JToken is in Array type, it is split and returned.
        /// </summary>
        /// <param name="jToken">JToken to split.</param>
        /// <param name="text">A string variable where the result will be stored.</param>
        /// <param name="transArray">An array required for reverse operation.</param>
        /// <param name="transIndex">Current index of <c>transArray</c>.</param>
        /// <param name="isReverse">Whether reverse operation is in progress.</param>
        /// <returns>Split JArray</returns>
        private JToken SplitJToken(JToken jToken, ref string text, ref string[] transArray, ref int transIndex, bool isReverse = false)
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
                        text += UnpackJToken(field);
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
                text += UnpackJToken(jToken);
                result = jToken;
            }

            return result;
        }

        private string UnpackJToken(JToken jToken)
        {
            string result = string.Empty;

            if (jToken.Type == JTokenType.Array)
            {
                foreach (JToken field in jToken)
                {
                    if (field.Type == JTokenType.Array)
                    {
                        string temp = UnpackJToken(field);

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
    }
}
