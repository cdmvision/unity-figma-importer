using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{
    [DisallowMultipleComponent]
    public class FigmaNode : MonoBehaviour
    {
        [SerializeField]
        private string _nodeID;

        public string nodeID
        {
            get => _nodeID;
            private set => _nodeID = value;
        }
        
        [SerializeField]
        private string _nodeName;

        public string nodeName
        {
            get => _nodeName;
            private set => _nodeName = value;
        }
        
        [SerializeField]
        private string _nodeType;

        public string nodeType
        {
            get => _nodeType;
            private set => _nodeType = value;
        }
        
        [SerializeField]
        private string _bindingKey;

        public string bindingKey
        {
            get => _bindingKey;
            private set => _bindingKey = value;
        }
        
        [SerializeField]
        private List<Styles.Style> _styles = new List<Styles.Style>();

        public List<Styles.Style> styles => _styles;

        // TODO: Should be private or at least internal. Because it is only available while importing figma file.
        public Node node { get; private set; }

        private RectTransform _rectTransform;

        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }

                return _rectTransform;
            }
        }
        
        public static T Create<T>(Node node) where T : FigmaNode
        {
            var go = new GameObject(node.name);
            var nodeObject = go.AddComponent<T>();

            nodeObject.node = node;
            nodeObject.nodeID = node.id;
            nodeObject.nodeName = node.name;
            nodeObject.nodeType = node.type;
            nodeObject.bindingKey = node.GetBindingName();
            nodeObject._rectTransform = nodeObject.gameObject.AddComponent<RectTransform>();
            
            if (node is SceneNode sceneNode)
            {
                nodeObject.gameObject.SetActive(sceneNode.visible);    
            }
            
            return nodeObject;
        }
    }
}