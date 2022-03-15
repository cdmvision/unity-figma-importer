using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public partial class Style
    {
        public override string ToString()
        {
            var style = new StringBuilder();

            var properties = GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.GetGetMethod() != null)
                {
                    var name = GetUssName(property.Name);

                    var valueString = "";
                    var value = property.GetValue(this);
                    if (value != null)
                    {
                        var propertyValue = value.GetType().GetProperties().First(p => p.Name == "value");
                        if (propertyValue.PropertyType.IsEnum)
                        {
                            valueString = GetUssValue(value.ToString());
                        }
                        else if (propertyValue.PropertyType == typeof(FontDefinition))
                        {
#if UNITY_EDITOR
                            Object asset = null;
                            
                            var fontDefinition = (FontDefinition) propertyValue.GetValue(value);
                            if (fontDefinition.fontAsset != null)
                            {
                                asset = fontDefinition.fontAsset;
                            }
                            else if (fontDefinition.font != null)
                            {
                                asset = fontDefinition.font;
                            }

                            if (asset != null)
                            {
                                var assetPath = UnityEditor.AssetDatabase.GetAssetPath(asset);
                                valueString = $"url(\"project:///{assetPath}\")";
                            }
#endif
                        }
                        else if(propertyValue.PropertyType == typeof(Background))
                        {
#if UNITY_EDITOR
                            Object asset = null;
                            
                            var background = (Background) propertyValue.GetValue(value);
                            if (background.vectorImage != null)
                            {
                                asset = background.vectorImage;
                            }
                            else if (background.sprite != null)
                            {
                                asset = background.sprite;
                            }
                            else if (background.texture != null)
                            {
                                asset = background.texture;
                            }
                            else if (background.renderTexture != null)
                            {
                                asset = background.renderTexture;
                            }
                            
                            if (asset != null)
                            {
                                var assetPath = UnityEditor.AssetDatabase.GetAssetPath(asset);
                                valueString = $"url(\"project:///{assetPath}\")";
                            }
#endif
                        }
                        else if (propertyValue.PropertyType == typeof(Rotate))
                        {
                            var r = (Rotate) propertyValue.GetValue(value);
                            valueString = $"{r.angle.ToString()}";
                        }
                        else if (propertyValue.PropertyType == typeof(UnityEngine.Color))
                        {
                            var c = (UnityEngine.Color) propertyValue.GetValue(value);
                            var c32 = (Color32) c;
                            valueString = $"rgba({c32.r}, {c32.g}, {c32.b}, {c.a:F1})";
                        }
                        else
                        {
                            valueString = value.ToString();
                        }
                    }

                    if (value != null && !string.IsNullOrWhiteSpace(valueString))
                    {
                        style.Append($"{name}: {valueString.ToLowerInvariant()}; ");
                    }
                }
            }

            return style.ToString();
        }

        private static string GetUssName(string name)
        {
            const string unity = "unity";

            var result = GetUssValue(name);

            // Unity types has to be start with '-' (i.e. -unity-text-overflow-position).
            if (result.Length >= unity.Length && result.Substring(0, unity.Length) == unity)
            {
                result = $"-{result}";
            }

            return result;
        }

        private static string GetUssValue(string value)
        {
            return string.Concat(value.Select((x, i) => char.IsUpper(x) && i != 0 ? $"-{x}" : x.ToString())).ToLower();
        }
    }
}