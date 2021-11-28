using UnityEngine;

namespace Cdm.Figma.UI
{
    public abstract class NodeConverter : ScriptableObject, INodeConverter
    {
        protected const string AssetMenuRoot = FigmaImporter.AssetMenuRoot;
        
        public abstract bool CanConvert(Node node, NodeConvertArgs args);
        public abstract NodeObject Convert(Node node, NodeConvertArgs args);
    }

    public abstract class NodeConverter<TNodeType> : NodeConverter where TNodeType : Node
    {
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            return node.GetType() == typeof(TNodeType);
        }
    }
}