using System;

namespace Cdm.Figma.Json
{
    public class ComponentPropertyJsonConverter : SubTypeJsonConverter<ComponentProperty, ComponentPropertyType>
    {
        protected override string GetTypeToken()
        {
            return nameof(ComponentProperty.type);
        }

        protected override bool TryGetActualType(ComponentPropertyType typeToken, out Type type)
        {
            switch (typeToken)
            {
                case ComponentPropertyType.InstanceSwap:
                    type = typeof(ComponentPropertyInstanceSwap);
                    return true;
                case ComponentPropertyType.Text:
                    type = typeof(ComponentPropertyText);
                    return true;
                case ComponentPropertyType.Boolean:
                    type = typeof(ComponentPropertyBoolean);
                    return true;
                case ComponentPropertyType.Variant:
                default:
                    type = typeof(ComponentProperty);
                    return true;
            }
        }
    }
}