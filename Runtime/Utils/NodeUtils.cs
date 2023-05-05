namespace Cdm.Figma.Utils
{
    public class NodeUtils
    {
        public static string HyphenateNodeID(string nodeId)
        {
            return $"{nodeId.Replace(":", "-").Replace(";", "_")}";
        }
    }
}