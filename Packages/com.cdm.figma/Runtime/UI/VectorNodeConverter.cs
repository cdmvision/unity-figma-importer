using System.IO;
using Unity.VectorGraphics;
using UnityEngine;

namespace Cdm.Figma.UI
{
    [CreateAssetMenu(fileName = nameof(VectorNodeConverter), 
        menuName = AssetMenuRoot + "Vector", order = AssetMenuOrder)]
    public class VectorNodeConverter : NodeConverter<VectorNode>
    {
        public static NodeObject Convert(VectorNode node, NodeConvertArgs args)
        {
            var nodeObject =  NodeObject.NewNodeObject(node, args);

            var absoluteBoundingBox = node.absoluteBoundingBox;
            var width = absoluteBoundingBox.width;
            var height = absoluteBoundingBox.height;

            // Override width and height if size property is exist.
            if (node.size != null)
            {
                width = node.size.x;
                height = node.size.y;
            }
            
            if (args.assets.TryGetValue(node.id, out var assetPath))
            {
                #if UNITY_EDITOR
                assetPath = Path.Combine("Assets", assetPath);
                Debug.Log(assetPath);
                
                var spriteGo = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (spriteGo != null)
                {
                    var svgImage = nodeObject.gameObject.AddComponent<SVGImage>();
                    svgImage.sprite = spriteGo.GetComponent<SpriteRenderer>().sprite;
                    
                    width = svgImage.sprite.rect.width;
                    height = svgImage.sprite.rect.height;
                }
                #endif
                
            }

            nodeObject.rectTransform.anchorMin = new Vector2(0, 1);
            nodeObject.rectTransform.anchorMax = new Vector2(0, 1);
            nodeObject.rectTransform.pivot = new Vector2(0, 1);
            nodeObject.rectTransform.localPosition = new Vector3(absoluteBoundingBox.x, -absoluteBoundingBox.y);
            nodeObject.rectTransform.sizeDelta = new Vector2(width, height);

            return nodeObject;
        }
        
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return Convert((VectorNode) node, args);
        }
    }
}