using System.Linq;
using UnityEngine;

namespace Cdm.Figma.UI
{
    [DisallowMultipleComponent]
    public class NodeObject : MonoBehaviour
    {
        [SerializeField]
        private string _nodeId;

        public string nodeId
        {
            get => _nodeId;
            set => _nodeId = value;
        }
        
        [SerializeField]
        private string _nodeName;

        public string nodeName
        {
            get => _nodeName;
            set => _nodeName = value;
        }
        
        [SerializeField]
        private string _nodeType;

        public string nodeType
        {
            get => _nodeType;
            set => _nodeType = value;
        }
        
        [SerializeField]
        private string _bindingId;

        public string bindingId
        {
            get => _bindingId;
            set => _bindingId = value;
        }
        
        [SerializeField]
        private string _localizationId;

        public string localizationId
        {
            get => _localizationId;
            set => _localizationId = value;
        }
        
        public RectTransform rectTransform { get; private set; }

        /// <summary>
        /// Initializes a new instance of the XElement class with the specified <paramref name="node"/>.
        /// </summary>
        public static NodeObject NewNodeObject(Node node, NodeConvertArgs args)
        {
            var go = new GameObject(node.name);
            var nodeObject = go.AddComponent<NodeObject>();

            nodeObject.nodeId = node.id;
            nodeObject.nodeName = node.name;
            nodeObject.nodeType = node.type;
            nodeObject.rectTransform = nodeObject.gameObject.AddComponent<RectTransform>();
            nodeObject.gameObject.SetActive(node.visible);


            var bindingId = GetSpecialId(node, args.importer.bindingPrefix);
            if (!string.IsNullOrEmpty(bindingId))
            {
                
                nodeObject.bindingId = bindingId;
            }
            
            var localizationId = GetSpecialId(node, args.importer.localizationPrefix);
            if (!string.IsNullOrEmpty(localizationId))
            {
                
                nodeObject.localizationId = localizationId;
            }

            return nodeObject;
        }
        
        private static string GetSpecialId(Node node, string prefix)
        {
            var tokens = node.name.Split(" ");
            var token = tokens.FirstOrDefault(t => t.StartsWith(prefix));
            return token?.Replace(prefix, "");
        }
    }
}