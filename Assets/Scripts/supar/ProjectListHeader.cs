using System;
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
            ByProject,
            ByDate,
            BySize
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

        private ToggleGroup _toggleGroup;

        public ProjectListHeaderItem.SortingMode GetSortingModeFor(SortingTarget sortingTarget)
        {
            switch (sortingTarget)
            {
                case SortingTarget.ByProject:
                    return _projectItem.sortingMode;
                case SortingTarget.ByDate:
                    return _dateItem.sortingMode;
                case SortingTarget.BySize:
                    return _sizeItem.sortingMode;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortingTarget), sortingTarget, null);
            }
        }

        public void OnBind(FigmaNode figmaNode)
        {
            var toggleGroup = figmaNode.gameObject.AddComponent<ToggleGroup>();
            toggleGroup.allowSwitchOff = false;

            _projectItem.mainToggle.group = toggleGroup;
            _dateItem.mainToggle.group = toggleGroup;
            _sizeItem.mainToggle.group = toggleGroup;
        }
        
        protected virtual void OnSortingModeChanged(SortingModeChangedEventArgs e)
        {
            sortingModeChanged?.Invoke(e);
        }

        public event Action<SortingModeChangedEventArgs> sortingModeChanged;
        
        public readonly struct SortingModeChangedEventArgs
        {
            public SortingTarget sortingTarget { get; }
            public ProjectListHeaderItem.SortingMode sortingMode { get; }

            public SortingModeChangedEventArgs(SortingTarget sortingTarget, 
                ProjectListHeaderItem.SortingMode sortingMode)
            {
                this.sortingTarget = sortingTarget;
                this.sortingMode = sortingMode;
            }
        }
    }
}