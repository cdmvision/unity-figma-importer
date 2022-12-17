using System;
using System.Collections.Generic;
using System.Linq;
using Cdm.Figma.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.SurfaceProjector.UI
{
    [FigmaNode("ProjectListHeader")]
    public class ProjectListHeader : MonoBehaviour, IFigmaNodeBinder
    {
        public enum SortingTarget
        {
            ByProjectName,
            ByDate,
            BySize
        }
        
        public enum SortingMode
        {
            None,
            Ascending,
            Descending
        }

        [SerializeField]
        [FigmaNode("@ProjectSortItem", isRequired = true)]
        private ProjectListHeaderItem _projectItem;
        
        [SerializeField]
        [FigmaNode("@DateSortItem", isRequired = true)]
        private ProjectListHeaderItem _dateItem;
        
        [SerializeField]
        [FigmaNode("@SizeSortItem", isRequired = true)]
        private ProjectListHeaderItem _sizeItem;

        [SerializeField, HideInInspector]
        private List<ProjectListHeaderItem> _items = new List<ProjectListHeaderItem>();
        
        private ToggleGroup _toggleGroup;

        private void Awake()
        {
            if (Application.isPlaying)
            {
                // Set sorting by date as default.
                _dateItem.sortingMode = SortingMode.Ascending;

                foreach (var item in _items)
                {
                    item.sortingModeChanged += HeaderItem_OnSortingModeChanged;
                }
            }
        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                foreach (var item in _items)
                {
                    item.sortingModeChanged -= HeaderItem_OnSortingModeChanged;
                }
            }
        }

        private void HeaderItem_OnSortingModeChanged(ProjectListHeaderItem.SortingModeChangedEventArgs e)
        {
            OnSortingModeChanged(new SortingModeChangedEventArgs(e.item.sortingTarget, e.sortingMode));
        }
        
        public void OnBind(FigmaNode figmaNode)
        {
            var toggleGroup = figmaNode.gameObject.AddComponent<ToggleGroup>();
            toggleGroup.allowSwitchOff = false;

            _projectItem.mainToggle.group = toggleGroup;
            _projectItem.sortingTarget = SortingTarget.ByProjectName;
            _items.Add(_projectItem);
            
            _dateItem.mainToggle.group = toggleGroup;
            _dateItem.sortingTarget = SortingTarget.ByDate;
            _items.Add(_dateItem);
            
            _sizeItem.mainToggle.group = toggleGroup;
            _sizeItem.sortingTarget = SortingTarget.BySize;
            _items.Add(_sizeItem);
        }
        
        public SortingMode GetSortingMode(SortingTarget sortingTarget)
        {
            return _items.First(x => x.sortingTarget == sortingTarget).sortingMode;
        }

        protected virtual void OnSortingModeChanged(SortingModeChangedEventArgs e)
        {
            Debug.Log($"OnSortingModeChanged({e.sortingTarget}, {e.sortingMode})");
            sortingModeChanged?.Invoke(e);
        }

        public event Action<SortingModeChangedEventArgs> sortingModeChanged;
        
        public readonly struct SortingModeChangedEventArgs
        {
            public SortingTarget sortingTarget { get; }
            public SortingMode sortingMode { get; }

            public SortingModeChangedEventArgs(SortingTarget sortingTarget, SortingMode sortingMode)
            {
                this.sortingTarget = sortingTarget;
                this.sortingMode = sortingMode;
            }
        }
    }
}