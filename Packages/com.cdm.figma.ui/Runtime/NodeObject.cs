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
        private string _bindingKey;

        public string bindingKey
        {
            get => _bindingKey;
            set => _bindingKey = value;
        }
        
        [SerializeField]
        private string _localizationKey;

        public string localizationKey
        {
            get => _localizationKey;
            set => _localizationKey = value;
        }
        
        public RectTransform rectTransform { get; private set; }
        
        public Node node { get; private set; }

        /// <summary>
        /// Initializes a new instance of the XElement class with the specified <paramref name="node"/>.
        /// </summary>
        public static NodeObject NewNodeObject(Node node, NodeConvertArgs args)
        {
            var go = new GameObject(node.name);
            var nodeObject = go.AddComponent<NodeObject>();

            nodeObject.node = node;
            nodeObject.nodeId = node.id;
            nodeObject.nodeName = node.name;
            nodeObject.nodeType = node.type;
            nodeObject.rectTransform = nodeObject.gameObject.AddComponent<RectTransform>();
            nodeObject.bindingKey = node.GetBindingName();
            nodeObject.localizationKey = node.GetLocalizationKey();
            
            if (node is SceneNode sceneNode)
            {
                nodeObject.gameObject.SetActive(sceneNode.visible);    
            }
            
            return nodeObject;
        }
    }
}