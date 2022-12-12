using Cdm.Figma.UI.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cdm.Figma.UI
{
    [RequireComponent(typeof(FigmaNode))]
    public class FigmaBehaviour : UIBehaviour
    {
        [SerializeField, HideInInspector]
        private FigmaNode _attachedNode;
        
        public FigmaNode attachedNode => _attachedNode;

        [SerializeField, HideInInspector]
        private bool _isBound = false;

        protected override void Awake()
        {
            base.Awake();

            if (!_isBound)
            {
                Bind();    
            }
        }

        public void Bind()
        {
            _attachedNode = GetComponent<FigmaNode>();
            var bindingResult = FigmaNodeBinder.Bind(this, _attachedNode);
            if (bindingResult.hasErrors)
            {
                Debug.LogException(new FigmaBindingFailedException(bindingResult.errors), this);
            }
            else
            {
                _isBound = true;    
            }
        }
    }
}