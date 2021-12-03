using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class ButtonComponentConverter : ComponentConverter<Button>
    {
        public ButtonComponentConverter()
        {
            typeId = "Button";
            properties = new List<ComponentProperty>()
            {
                new ComponentProperty()
                {
                    key = "State",
                    variants = new ComponentVariant[]
                    {
                        new ComponentVariant(ComponentState.Default, ComponentState.Default),
                        new ComponentVariant(ComponentState.Hover, ComponentState.Hover),
                        new ComponentVariant(ComponentState.Press, ComponentState.Press),
                        new ComponentVariant(ComponentState.Disabled, ComponentState.Disabled),
                    }
                }
            };
        }

        public override NodeData Convert(Node node, NodeConvertArgs args)
        {
            var instanceNode = (InstanceNode) node;
            
            /*if (args.fileContent.components.TryGetValue(instanceNode.componentId, out var component))
            {
                var componentSet = args.componentSets.FirstOrDefault(c => c.id == component.componentSetId);
                if (componentSet != null)
                {
                    var variants = componentSet.children;
                    foreach (var variant in variants)
                    {
                        var componentVariant = (ComponentNode) variant;
                        
                        // TODO: Check variant property: componentVariant.name
                        // State=Default

                        foreach (var property in properties)
                        {
                            foreach (var propertyVariant in property.variants)
                            {
                                if ($"{property.key}={propertyVariant.value}" == componentVariant.name)
                                {
                                    if (propertyVariant.key == ComponentState.Default)
                                    {
                                        
                                    }
                                    else if (propertyVariant.key == ComponentState.Hover)
                                    {
                                        
                                    }
                                    // TODO: rest of it
                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("Component set does not exist!");
                }
            }*/

            return NodeData.New<Button>(node, args);
        }
    }
}