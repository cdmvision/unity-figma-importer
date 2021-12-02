using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(ButtonComponentConverter), menuName = AssetMenuRoot + "Button", order = AssetMenuOrder)]
    public class ButtonComponentConverter : ComponentConverter
    {
        protected override string GetDefaultTypeId()
        {
            return "Button";
        }

        protected override ISet<ComponentProperty> GetVariants()
        {
            return new HashSet<ComponentProperty>()
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

        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            var instanceNode = (InstanceNode) node;
            
            if (args.fileContent.components.TryGetValue(instanceNode.componentId, out var component))
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
            }

            throw new NotImplementedException();
        }
    }
}