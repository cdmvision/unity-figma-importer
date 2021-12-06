using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class ScriptGenerator : IScriptGenerator
    {
        public const string DefaultNamespaceName = "Cdm.Figma.UIToolkit.Documents";
        
        public string namespaceName { get; set; } = DefaultNamespaceName;
        public string className { get; set; }
        
        public GeneratedScript Generate(NodeElement page)
        {
            var properties = new Dictionary<string, Type>();
            PopulateClassPropertiesRecurse(page, properties);

            if (string.IsNullOrEmpty(className))
            {
                if (!TryFormatAsClassName(page.node.name, out var name))
                    throw new Exception($"Class name could not be generated from this '{page.node.name}'.");

                className = name;
            }

            var scriptText = new StringBuilder();
            scriptText.AppendLine($"using UnityEngine.UIElements;");
            scriptText.AppendLine();

            if (!string.IsNullOrEmpty(namespaceName))
            {
                scriptText.AppendLine($"namespace {namespaceName}");    
                scriptText.AppendLine($"{{");    
            }
            
            scriptText.AppendLine($"public class {className} : UnityEngine.MonoBehaviour");
            scriptText.AppendLine($"{{");
            
            scriptText.AppendLine($"\tpublic {typeof(UIDocument).FullName} document;");
            scriptText.AppendLine();
            
            // Write all properties.
            if (properties.Any())
            {
                foreach (var (name, type) in properties)
                {
                    scriptText.AppendLine($"\tpublic {type.FullName} {name} {{ get; private set; }}");
                }
                scriptText.AppendLine();
            }

            scriptText.AppendLine($"\tprivate void Awake()");
            scriptText.AppendLine($"\t{{");
            
            // Initialize all properties.
            foreach (var (name, type) in properties)
            {
                
                scriptText.AppendLine($"\t\t{name} = document.{nameof(UIDocument.rootVisualElement)}.Q<{type.FullName}>(\"{name}\");");
            }
            
            scriptText.AppendLine($"\t}}");
            scriptText.AppendLine($"}}");

            if (!string.IsNullOrEmpty(namespaceName))
            {
                scriptText.AppendLine($"}}");    
            }

            return new GeneratedScript(className, scriptText.ToString());
        }
        
        private static void PopulateClassPropertiesRecurse(NodeElement node, Dictionary<string, Type> properties)
        {
            if (!string.IsNullOrEmpty(node.elementName))
            {
                if (properties.ContainsKey(node.elementName))
                    throw new Exception($"Duplicate element name does not allowed '{node.elementName}'.");
                
                properties.Add(node.elementName, node.elementType);
            }
            
            foreach (var child in node.children)
            {
                PopulateClassPropertiesRecurse(child, properties);
            }
        }

        private static bool TryFormatAsClassName(string input, out string className)
        {
            input = input.Replace(" ", "");

            if (input.Length > 0)
            {
                if (char.IsLetter(input[0]))
                {
                    className = input;
                    return true;
                }
            }

            className = null;
            return false;
        }
    }
}