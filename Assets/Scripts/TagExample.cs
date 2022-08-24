using System;
using System.Linq;
using Cdm.Figma.UI;
using UnityEngine;

public class TagExample : MonoBehaviour
{
    public Canvas canvas;
    public FigmaPage page;

    public Device device;
    
    private void Awake()
    {
        if (page != null)
        {
            var pageInstance = Instantiate(page, canvas.transform);
            pageInstance.TraverseDfs(node =>
            {
                if (node.tags.Length > 0 && !node.tags.Contains(device.ToString().ToLowerInvariant()))
                {
                    Destroy(node.gameObject);
                    return false;
                }
                
                return true;
            });
        }
        else
        {
            Debug.LogWarning("Page is not assigned.");
        }
    }
    
    [Serializable]
    public enum Device
    {
        iPhone,
        iPad
    }
}