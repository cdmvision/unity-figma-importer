using Cdm.Figma;
using Cdm.Figma.UI;
using Cdm.Figma.UI.Utils;

//[FigmaComponentConverter]
public class CustomInputFieldComponentConverter : ComponentConverter
{
    protected override bool CanConvertType(string typeId)
    {
        return typeId == "InputFieldWithClearButton";
    }

    protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
    {
        var nodeObject =  base.Convert(parentObject, instanceNode, args);
        if (nodeObject != null)
        {
            
            var inputField = nodeObject.gameObject.AddComponent<CustomInputField>();
            FigmaNodeBinder.Bind(inputField, nodeObject);
            return nodeObject;
        }

        return null;
    }
}