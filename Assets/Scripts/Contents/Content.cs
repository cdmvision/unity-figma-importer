using UnityEngine;

namespace Cdm.Figma.Examples
{
    public class Content : MonoBehaviour
    {
        protected virtual void Start()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }

    }
}