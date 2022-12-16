using System;
using Cdm.Figma.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.SurfaceProjector.UI
{
    [FigmaComponent("ProjectListHeaderItem")]
    public class ProjectListHeaderItem : MonoBehaviour
    {
        public enum SortingMode
        {
            None,
            Ascending,
            Descending
        }

        [SerializeField]
        [FigmaNode("@MainToggle", isRequired = true)]
        private Toggle _mainToggle;

        public Toggle mainToggle => _mainToggle;
        
        [SerializeField]
        [FigmaNode("@SortToggle", isRequired = true)]
        private Toggle _sortToggle;

        [SerializeField]
        private SortingMode _sortingMode = SortingMode.None;

        public SortingMode sortingMode
        {
            get => _sortingMode;
            set
            {
                _sortingMode = value;
                UpdateSortingMode();
            }
        }

        private void Awake()
        {
            if (Application.isPlaying)
            {
                sortingMode = _sortingMode;

                _mainToggle.onValueChanged.AddListener(OnToggleValueChanged);
                _sortToggle.onValueChanged.AddListener(OnToggleValueChanged);    
            }
        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                _mainToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
                _sortToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            }
        }

        private void OnToggleValueChanged(bool isOn)
        {
            switch (sortingMode)
            {
                case SortingMode.None:
                    sortingMode = SortingMode.Ascending;
                    break;
                case SortingMode.Ascending:
                    sortingMode = SortingMode.Descending;
                    break;
                case SortingMode.Descending:
                    sortingMode = SortingMode.Ascending;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateSortingMode()
        {
            _mainToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            _mainToggle.isOn = sortingMode != SortingMode.None;
            _mainToggle.onValueChanged.AddListener(OnToggleValueChanged);

            _sortToggle.gameObject.SetActive(_mainToggle.isOn);

            _sortToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            _sortToggle.isOn = sortingMode == SortingMode.Descending;
            _sortToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                sortingMode = _sortingMode;
            }
        }
    }
}