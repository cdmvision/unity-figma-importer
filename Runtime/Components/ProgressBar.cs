using System;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Components
{
    [FigmaComponent("ProgressBar")]
    public class ProgressBar : UIBehaviour, IFigmaNodeBinder
    {
        [FigmaNode("@Fill")]
        [SerializeField, HideInInspector]
        private Image _fill;

        [SerializeField, Range(0f, 1f)]
        private float _value = 0f;

        public float value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp01(value);

                if (_fill != null)
                {
                    _fill.fillAmount = _value;
                }
            }
        }

        public void OnBind(FigmaNode node)
        {
            if (_fill == null)
                return;

            // Set default values.
            _fill.type = Image.Type.Filled;
            _fill.fillMethod = Image.FillMethod.Horizontal;
            _fill.fillOrigin = (int) Image.OriginHorizontal.Left;
            
            if (node.node.TryGetPluginData(out var pluginData))
            {
                var componentData = pluginData.GetComponentDataAs<ComponentData>();
                if (componentData != null)
                {
                    _fill.fillMethod = componentData.direction;
                    _fill.fillClockwise = componentData.clockwise;

                    switch (componentData.direction)
                    {
                        case Image.FillMethod.Horizontal:
                            _fill.fillOrigin = (int)componentData.originHorizontal;
                            break;
                        case Image.FillMethod.Vertical:
                            _fill.fillOrigin = (int)componentData.originVertical;
                            break;
                        case Image.FillMethod.Radial90:
                            _fill.fillOrigin = (int)componentData.originRadial90;
                            break;
                        case Image.FillMethod.Radial180:
                            _fill.fillOrigin = (int)componentData.originRadial180;
                            break;
                        case Image.FillMethod.Radial360:
                            _fill.fillOrigin = (int)componentData.originRadial360;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            value = _value;
        }
#endif

        [DataContract]
        public class ComponentData
        {
            [DataMember]
            public Image.FillMethod direction { get; set; } = Image.FillMethod.Horizontal;

            [DataMember]
            public Image.OriginHorizontal originHorizontal { get; set; } = Image.OriginHorizontal.Left;

            [DataMember]
            public Image.OriginVertical originVertical { get; set; } = Image.OriginVertical.Bottom;

            [DataMember]
            public Image.Origin90 originRadial90 { get; set; } = Image.Origin90.BottomLeft;

            [DataMember]
            public Image.Origin180 originRadial180 { get; set; } = Image.Origin180.Bottom;

            [DataMember]
            public Image.Origin360 originRadial360 { get; set; } = Image.Origin360.Bottom;

            [DataMember]
            public bool clockwise { get; set; } = false;
        }
    }
}