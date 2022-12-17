using System;
using Cdm.Figma.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.SurfaceProjector.UI
{
    [FigmaComponent("ProjectListHeaderItem")]
    public class ProjectListHeaderItem : MonoBehaviour
    {
        [SerializeField]
        [FigmaNode("@MainToggle", isRequired = true)]
        private Toggle _mainToggle;

        public Toggle mainToggle => _mainToggle;
        
        [SerializeField]
        [FigmaNode("@SortToggle", isRequired = true)]
        private Toggle _sortToggle;

        [SerializeField]
        private ProjectListHeader.SortingTarget _sortingTarget = ProjectListHeader.SortingTarget.ByProjectName;

        public ProjectListHeader.SortingTarget sortingTarget
        {
            get => _sortingTarget;
            set => _sortingTarget = value;
        }
        
        [SerializeField]
        private ProjectListHeader.SortingMode _sortingMode = ProjectListHeader.SortingMode.None;

        public ProjectListHeader.SortingMode sortingMode
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
                case ProjectListHeader.SortingMode.None:
                    sortingMode = ProjectListHeader.SortingMode.Ascending;
                    break;
                case ProjectListHeader.SortingMode.Ascending:
                    sortingMode = ProjectListHeader.SortingMode.Descending;
                    break;
                case ProjectListHeader.SortingMode.Descending:
                    sortingMode = ProjectListHeader.SortingMode.Ascending;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateSortingMode()
        {
            _mainToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            _mainToggle.isOn = sortingMode != ProjectListHeader.SortingMode.None;
            _mainToggle.onValueChanged.AddListener(OnToggleValueChanged);

            _sortToggle.gameObject.SetActive(_mainToggle.isOn);

            _sortToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            _sortToggle.isOn = sortingMode == ProjectListHeader.SortingMode.Descending;
            _sortToggle.onValueChanged.AddListener(OnToggleValueChanged);

            OnSortingModeChanged(new SortingModeChangedEventArgs(this, sortingMode));
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                sortingMode = _sortingMode;
            }
        }
        
        protected virtual void OnSortingModeChanged(SortingModeChangedEventArgs e)
        {
            sortingModeChanged?.Invoke(e);
        }

        public event Action<SortingModeChangedEventArgs> sortingModeChanged;
        
        public readonly struct SortingModeChangedEventArgs
        {
            public ProjectListHeaderItem item { get; }
            public ProjectListHeader.SortingMode sortingMode { get; }

            public SortingModeChangedEventArgs(ProjectListHeaderItem item, ProjectListHeader.SortingMode sortingMode)
            {
                this.item = item;
                this.sortingMode = sortingMode;
            }
        }
    }
}