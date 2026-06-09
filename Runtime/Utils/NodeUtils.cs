namespace Cdm.Figma.Utils
{
    public abstract class NodeUtils
    {
        public static string HyphenateNodeID(string nodeId)
        {
            return $"{nodeId.Replace(":", "-").Replace(";", "_")}";
        }
    }
}