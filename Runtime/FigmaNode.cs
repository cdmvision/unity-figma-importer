using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{
    [DisallowMultipleComponent]
    public class FigmaNode : MonoBehaviour, IEnumerable<FigmaNode>
    {
        [SerializeField, HideInInspector]
        private FigmaDesign _figmaDesign;

        public FigmaDesign figmaDesign
        {
            get => _figmaDesign;
            internal set => _figmaDesign = value;
        }

        [SerializeField]
        private string _nodeId;

        /// <inheritdoc cref="Node.id"/>
        public string nodeId
        {
            get => _nodeId;
            private set => _nodeId = value;
        }

        [SerializeField]
        private string _nodeName;

        /// <inheritdoc cref="Node.name"/>
        public string nodeName
        {
            get => _nodeName;
            private set => _nodeName = value;
        }

        [SerializeField]
        private string _nodeType;

        /// <inheritdoc cref="Node.type"/>
        public string nodeType
        {
            get => _nodeType;
            private set => _nodeType = value;
        }

        [SerializeField]
        private string _bindingKey;

        /// <summary>
        /// The binding key set by the user in Figma editor using Unity Figma Plugin. It can be used to access
        /// to the node, especially at runtime.
        /// </summary>
        public string bindingKey
        {
            get => _bindingKey;
            private set => _bindingKey = value;
        }

        [SerializeField]
        private string[] _tags = Array.Empty<string>();

        /// <summary>
        /// The node tags added by the user in Figma editor using Unity Figma Plugin. It can be used for filtering
        /// nodes.
        /// </summary>
        public string[] tags
        {
            get => _tags;
            private set => _tags = (value ?? Array.Empty<string>());
        }

        [SerializeField]
        private List<FigmaImporterLog> _logs = new List<FigmaImporterLog>();

        public List<FigmaImporterLog> logs => _logs;

        /// <summary>
        /// The actual figma node that is created from.
        /// </summary>
        /// <remarks>
        /// It is available only while importing figma file.
        /// </remarks>
        internal Node node { get; private set; }

        /// <summary>
        /// It points to an instance node that will be used when converting a component node for
        /// <see cref="ComponentPropertyType.InstanceSwap"/> feature.
        /// </summary>
        /// <remarks>
        /// It can be available only while importing figma file.
        /// </remarks>
        internal Node referenceNode { get; set; }

        /// <summary>
        /// All node styles that will be applied while conversion process.
        /// </summary>
        /// <remarks>
        /// It is available only while importing figma file.
        /// </remarks>
        public List<Styles.Style> styles { get; } = new List<Styles.Style>();

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

        public static T Create<T>(Node node, GameObject existingGameObject = null) where T : FigmaNode
        {
            if (existingGameObject == null)
            {
                existingGameObject = new GameObject(node.name);
            }

            var nodeObject = existingGameObject.AddComponent<T>();

            nodeObject.node = node;
            nodeObject.nodeId = node.id;
            nodeObject.nodeName = node.name;
            nodeObject.nodeType = node.type;
            nodeObject.bindingKey = node.GetBindingKey();
            nodeObject.tags = node.GetTags();

            nodeObject._rectTransform = nodeObject.gameObject.AddComponent<RectTransform>();

            if (node is SceneNode sceneNode)
            {
                nodeObject.gameObject.SetActive(sceneNode.visible);
            }

            return nodeObject;
        }

        public virtual IEnumerator<FigmaNode> GetEnumerator()
        {
            foreach (Transform child in transform)
            {
                var childNode = child.GetComponent<FigmaNode>();
                if (childNode != null)
                {
                    yield return childNode;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return $"[{nodeId}, '{name}']";
        }
    }
}