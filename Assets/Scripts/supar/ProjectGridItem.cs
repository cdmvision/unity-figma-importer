using Cdm.Figma.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.SurfaceProjector.UI
{
    [FigmaNode("ProjectGridItem")]
    public class ProjectGridItem : MonoBehaviour
    {
        [FigmaNode("@ContextMenuButton")]
        [SerializeField]
        private Button _contextMenuButton;
        
        [FigmaNode("@ContextMenuButton")]
        [SerializeField]
        private Button _mainToggle;
        
        [FigmaNode("@Title")]
        [SerializeField]
        private TextMeshProUGUI _title;
        
        [FigmaNode("@Subtitle")]
        [SerializeField]
        private TextMeshProUGUI _subtitle;

        private void Start()
        {
            _title.text = "Bewer 201d ad65s3";
            _subtitle.text = "C:/Users/hip/adsadasdsads/sdasd/sadsa/dasd.spproject";
            
            _mainToggle.onClick.AddListener(() => Debug.Log("Loading a project..."));
            _contextMenuButton.onClick.AddListener(() => Debug.Log("Context menu openging"));
        }
    }
}