using System;

namespace Cdm.Figma.Utils
{
    public class ComponentPropertyDefinitionJsonConverter
        : SubTypeJsonConverter<ComponentPropertyDefinition, ComponentPropertyType>
    {
        protected override string GetTypeToken()
        {
            return nameof(ComponentPropertyDefinition.type);
        }

        protected override bool TryGetActualType(ComponentPropertyType typeToken, out Type type)
        {
            switch (typeToken)
            {
                case ComponentPropertyType.InstanceSwap:
                    type = typeof(ComponentPropertyDefinitionInstanceSwap);
                    return true;
                case ComponentPropertyType.Text:
                    type = typeof(ComponentPropertyDefinitionText);
                    return true;
                case ComponentPropertyType.Boolean:
                    type = typeof(ComponentPropertyDefinitionBoolean);
                    return true;
                case ComponentPropertyType.Variant:
                    type = typeof(ComponentPropertyDefinitionVariant);
                    return true;
                default:
                    type = typeof(ComponentPropertyDefinition);
                    return true;
            }
        }
    }
}