using System.Xml.Linq;
using UnityEngine;

namespace Cdm.Figma
{
    public class SvgImporter
    {
        public static string Import(VectorNode node)
        {
            var rect = node.absoluteBoundingBox;
            var width = Mathf.CeilToInt(rect.width);
            var height = Mathf.CeilToInt(rect.height);
            
            var xml = new XDocument();
            var svg = new XElement("svg");
            svg.Add(new XAttribute("width", width));
            svg.Add(new XAttribute("height", height));
            svg.Add(new XAttribute("viewBox", $"0 0 {width} {height}"));
            svg.Add(new XAttribute("fill", "none"));
            //svg.Add(new XAttribute("xmlns", "http://www.w3.org/2000/svg"));
            xml.Add(svg);

            
            if (node.fillGeometry != null)
            {
                foreach (var path in node.fillGeometry)
                {
                    var pathFill = new XElement("path");
                    if (!string.IsNullOrEmpty(path.path))
                    {
                        pathFill.Add(new XAttribute("d", path.path));    
                        pathFill.Add(new XAttribute("fill", "white"));    
                    }
                    svg.Add(pathFill);
                }
            }
            
            
            if (node.strokeGeometry != null)
            {
                foreach (var path in node.strokeGeometry)
                {
                    if (!string.IsNullOrEmpty(path.path))
                    {
                        var pathStroke = new XElement("path");
                        pathStroke.Add(new XAttribute("d", path.path));
                        pathStroke.Add(new XAttribute("stroke", "white"));

                        if (node.strokeWeight.HasValue)
                        {
                            pathStroke.Add(new XAttribute("stroke-width", node.strokeWeight.Value));    
                        }
                        svg.Add(pathStroke);
                    }
                }
            }

            return xml.ToString();
        }
    }
}