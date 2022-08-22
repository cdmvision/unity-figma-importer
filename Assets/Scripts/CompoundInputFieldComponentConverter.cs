using Cdm.Figma;
using Cdm.Figma.UI;

[FigmaComponentConverter]
public class CompoundInputFieldComponentConverter : ComponentConverter
{
    protected override bool CanConvertType(string typeID)
    {
        return typeID == "InputFieldWithClearButton";
    }

    protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
    {
        var nodeObject =  base.Convert(parentObject, instanceNode, args);
        if (nodeObject != null)
        {
            var compoundInputField = nodeObject.gameObject.AddComponent<CompoundInputField>();
            compoundInputField.Resolve();
            return nodeObject;
        }

        return null;
    }
}