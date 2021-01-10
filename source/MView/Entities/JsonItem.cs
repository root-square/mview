using MahApps.Metro.IconPacks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MView.Entities
{
    public class JsonItem
    {
        #region ::Constructors::

        public JsonItem()
        {

        }

        public JsonItem(JToken json, int number)
        {

        }

        public JsonItem(JToken json, bool isExapnded = false)
        {
            SubItems = new List<JsonItem>();

            if (json.Type == JTokenType.Object)
            {
                Type = GetJsonItemType(json.Type);
                Icon = GetIcon(Type);
                Name = "Object";
                Path = json.Path;
                Value = json;
                IsExpanded = isExapnded;

                foreach (JToken token in json.Children())
                {
                    SubItems.Add(new JsonItem(token));
                }
            }
            else if (json.Type == JTokenType.Array)
            {
                Type = GetJsonItemType(json.Type);
                Icon = GetIcon(Type);
                Name = "Array";
                Path = json.Path;
                Value = json;
                IsExpanded = isExapnded;

                // Set array item number.
                int count = 0;

                foreach (JToken token in json.Children())
                {
                    JsonItem item = new JsonItem(token);
                    item.Name = $"{count++} : {item.Name}";
                    SubItems.Add(item);
                }
            }
            else if (json.Type == JTokenType.Property)
            {
                JProperty property = json.ToObject<JProperty>();

                Type = GetJsonItemType(property.Value.Type);
                Icon = GetIcon(Type);
                Path = json.Path;
                Value = property.Value;
                IsExpanded = isExapnded;

                // Collect property value.
                if (property.Value.Type == JTokenType.Object)
                {
                    JsonItem objValue = new JsonItem(property.Value);
                    Name = property.Name;
                    SubItems = objValue.SubItems;
                }
                else if (property.Value.Type == JTokenType.Array)
                {
                    JsonItem arrayValue = new JsonItem(property.Value);
                    Name = property.Name;
                    SubItems = arrayValue.SubItems;
                }
                else if (property.Value.Type == JTokenType.Bytes || property.Value.Type == JTokenType.Float || property.Value.Type == JTokenType.Integer)
                {
                    Name = $"{property.Name} : {property.Value}";
                }
                else if (property.Value.Type == JTokenType.String)
                {
                    Name = $"{property.Name} : \"{property.Value}\"";
                }
                else if (property.Value.Type == JTokenType.Boolean)
                {
                    Name = $"{property.Name} : {(bool)property.Value}";
                }
                else if (property.Value.Type == JTokenType.Null)
                {
                    Name = $"{property.Name} : null";
                }
                else
                {
                    Name = $"{property.Name} : {property.Value}";
                }
            }
            else if (json.Type == JTokenType.String)
            {
                Type = GetJsonItemType(json.Type);
                Icon = GetIcon(Type);
                Name = $"\"{json}\"";
                Path = json.Path;
                Value = json;
                IsExpanded = isExapnded;
            }
            else if (json.Type == JTokenType.Bytes || json.Type == JTokenType.Float || json.Type == JTokenType.Integer)
            {
                Type = GetJsonItemType(json.Type);
                Icon = GetIcon(Type);
                Name = json.ToString();
                Path = json.Path;
                Value = json;
                IsExpanded = isExapnded;
            }
            else if (json.Type == JTokenType.Boolean)
            {
                Type = GetJsonItemType(json.Type);
                Icon = GetIcon(Type);
                Name = $"{(bool)json}";
                Path = json.Path;
                Value = json;
                IsExpanded = isExapnded;
            }
            else if (json.Type == JTokenType.Null)
            {
                Type = GetJsonItemType(json.Type);
                Icon = GetIcon(Type);
                Name = "null";
                Path = json.Path;
                
                Value = JValue.CreateNull();
                IsExpanded = isExapnded;
            }
            else
            {
                Type = GetJsonItemType(json.Type);
                Icon = GetIcon(Type);
                Name = $"{json}";
                Path = json.Path;
                Value = json;
                IsExpanded = isExapnded;
            }
        }

        #endregion

        #region ::Methods::

        private JsonItemType GetJsonItemType(JTokenType type)
        {
            if (type == JTokenType.Bytes || type == JTokenType.Float || type == JTokenType.Integer)
            {
                return JsonItemType.Number;
            }
            else if (type == JTokenType.String)
            {
                return JsonItemType.String;
            }
            else if (type == JTokenType.Boolean)
            {
                return JsonItemType.Boolean;
            }
            else if (type == JTokenType.Array)
            {
                return JsonItemType.Array;
            }
            else if (type == JTokenType.Object)
            {
                return JsonItemType.Object;
            }
            else if (type == JTokenType.Null)
            {
                return JsonItemType.Null;
            }
            else
            {
                return JsonItemType.None;
            }
        }

        private PackIconMaterialKind GetIcon(JsonItemType type)
        {
            if (type == JsonItemType.Object)
            {
                return PackIconMaterialKind.CodeBraces;
            }
            else if (type == JsonItemType.Array)
            {
                return PackIconMaterialKind.CodeBrackets;
            }
            else
            {
                return PackIconMaterialKind.SquareRounded;
            }
        }

        private SolidColorBrush GetIconColor(JsonItemType type)
        {
            if (type == JsonItemType.Object)
            {
                return new SolidColorBrush(Color.FromRgb(31, 58, 147));
            }
            else if (type == JsonItemType.Array)
            {
                return new SolidColorBrush(Color.FromRgb(31, 58, 147));
            }
            else if (type == JsonItemType.String)
            {
                return new SolidColorBrush(Color.FromRgb(31, 58, 147));
            }
            else if (type == JsonItemType.Number)
            {
                return new SolidColorBrush(Color.FromRgb(0, 177, 106));
            }
            else if (type == JsonItemType.Boolean)
            {
                return new SolidColorBrush(Color.FromRgb(31, 58, 147));
            }
            else if (type == JsonItemType.Null)
            {
                return new SolidColorBrush(Color.FromRgb(207, 0, 15));
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        #endregion

        #region ::Properties::

        public JsonItemType Type { get; set; }

        public PackIconMaterialKind Icon { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public JToken Value { get; set; }

        public bool IsExpanded { get; set; }

        public List<JsonItem> SubItems { get; set; }

        #endregion
    }
}
