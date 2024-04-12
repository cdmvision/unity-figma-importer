using Cdm.Figma.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.Examples
{
    [FigmaNode("@LayoutsContent")]
    public class LayoutsContent : ScrollableContent
    {
        [FigmaNode("@Layouts")]
        [SerializeField]
        private RectTransform _layouts;
        
        [FigmaNode("@HorizontalLayout")]
        [SerializeField]
        private RectTransform _horizontalLayout;
        
        [FigmaNode("@VerticalLayout")]
        [SerializeField]
        private RectTransform _verticalLayout;
        
        [FigmaNode("@GridLayout")]
        [SerializeField]
        private RectTransform _gridLayout;
        
        [FigmaNode("@AddHorizontalElement")]
        [SerializeField]
        private Button _addHorizontalElement;
        
        [FigmaNode("@AddVerticalElement")]
        [SerializeField]
        private Button _addVerticalElement;
        
        [FigmaNode("@AddGridElement")]
        [SerializeField]
        private Button _addGridElement;
        
        [FigmaNode("@RemoveHorizontalElement")]
        [SerializeField]
        private Button _removeHorizontalElement;
        
        [FigmaNode("@RemoveVerticalElement")]
        [SerializeField]
        private Button _removeVerticalElement;
        
        [FigmaNode("@RemoveGridElement")]
        [SerializeField]
        private Button _removeGridElement;

        private void Awake()
        {
            _addHorizontalElement.onClick.AddListener(AddHorizontalElement);
            _addVerticalElement.onClick.AddListener(AddVerticalElement);
            _addGridElement.onClick.AddListener(AddGridElement);
            _removeHorizontalElement.onClick.AddListener(RemoveHorizontalElement);
            _removeVerticalElement.onClick.AddListener(RemoveVerticalElement);
            _removeGridElement.onClick.AddListener(RemoveGridElement);
            
            _removeHorizontalElement.gameObject.SetActive(false);
            _removeVerticalElement.gameObject.SetActive(false);
            _removeGridElement.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _addHorizontalElement.onClick.RemoveAllListeners();
            _addVerticalElement.onClick.RemoveAllListeners();
            _addGridElement.onClick.RemoveAllListeners();
            _removeHorizontalElement.onClick.RemoveAllListeners();
            _removeVerticalElement.onClick.RemoveAllListeners();
            _removeGridElement.onClick.RemoveAllListeners();
        }

        private void AddGridElement()
        {
            var child = _gridLayout.transform.GetChild(0);
            if (_gridLayout.childCount < 6)
            {
                Instantiate(child, _gridLayout);
            }

            if (_gridLayout.childCount >= 6)
            {
                _addGridElement.gameObject.SetActive(false);
                _removeGridElement.gameObject.SetActive(true);
            }
        }

        private void AddVerticalElement()
        {
            var child = _verticalLayout.transform.GetChild(0);
            if (_verticalLayout.childCount < 5)
            {
                Instantiate(child, _verticalLayout);
            }

            if (_verticalLayout.childCount >= 5)
            {
                _addVerticalElement.gameObject.SetActive(false);
                _removeVerticalElement.gameObject.SetActive(true);
            }
        }

        private void AddHorizontalElement()
        {
            var child = _horizontalLayout.transform.GetChild(0);
            if (_horizontalLayout.childCount < 5)
            {
                Instantiate(child, _horizontalLayout);
            }

            if (_horizontalLayout.childCount >= 5)
            {
                _addHorizontalElement.gameObject.SetActive(false);
                _removeHorizontalElement.gameObject.SetActive(true);
            }
        }

        private void RemoveGridElement()
        {
            var child = _gridLayout.transform.GetChild(_gridLayout.childCount-1);
            if (_gridLayout.childCount >= 3)
            {
                Destroy(child.gameObject);
            }

            if (_gridLayout.childCount <= 3)
            {
                _addGridElement.gameObject.SetActive(true);
                _removeGridElement.gameObject.SetActive(false);
            }
        }

        private void RemoveVerticalElement()
        {
            var child = _verticalLayout.transform.GetChild(_verticalLayout.childCount-1);
            if (_verticalLayout.childCount >= 4)
            {
                Destroy(child.gameObject);
            }

            if (_verticalLayout.childCount <= 4)
            {
                _addVerticalElement.gameObject.SetActive(true);
                _removeVerticalElement.gameObject.SetActive(false);
            }
        }

        private void RemoveHorizontalElement()
        {
            var child = _horizontalLayout.transform.GetChild(_horizontalLayout.childCount-1);
            if (_horizontalLayout.childCount >= 4)
            {
                Destroy(child.gameObject);
            }
            
            if (_horizontalLayout.childCount <= 4)
            {
                _addHorizontalElement.gameObject.SetActive(true);
                _removeHorizontalElement.gameObject.SetActive(false);
            }
        }

        protected override RectTransform GetContent()
        {
            return _layouts;
        }
    }
}