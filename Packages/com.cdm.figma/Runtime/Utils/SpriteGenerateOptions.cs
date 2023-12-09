using Unity.VectorGraphics;
using UnityEngine;

namespace Cdm.Figma.Utils
{
    public struct SpriteGenerateOptions
    {
        public VectorUtils.TessellationOptions tessellationOptions { get; set; }

        /// <inheritdoc cref="Texture.filterMode"/>
        public FilterMode filterMode { get; set; }

        /// <inheritdoc cref="Texture.wrapMode"/>
        public TextureWrapMode wrapMode { get; set; }

        /// <summary>
        /// The minimum size of the texture holding sprite data.
        /// </summary>
        public int minTextureSize { get; set; }

        /// <summary>
        /// The maximum size of the texture holding sprite data.
        /// </summary>
        public int maxTextureSize { get; set; }
        
        /// <summary>
        /// The resolution of the texture for rectangle nodes without a gradient color
        /// (<see cref="RectangleNode"/> and <see cref="FrameNode"/> etc).
        /// </summary>
        public int rectTextureSize { get; set; }

        /// <summary>
        /// The number of samples per pixel for anti-aliasing.
        /// </summary>
        public int sampleCount { get; set; }

        /// <summary>
        /// The maximum size of the texture holding gradient data.
        /// </summary>
        public ushort gradientResolution { get; set; }

        /// <summary>
        /// How many SVG "pixels" map into a Unity unit.
        /// </summary>
        public float pixelsPerUnit { get; set; }

        public float scaleFactor { get; set; }
        public SceneNode overrideNode { get; set; }

        /// <summary>
        /// When true, expand the edges to avoid a dark banding effect caused by filtering.
        /// This is slower to render and uses more graphics memory.
        /// </summary>
        public bool expandEdges { get; set; }

        public static SpriteGenerateOptions GetDefault()
        {
            return new SpriteGenerateOptions()
            {
                tessellationOptions = new VectorUtils.TessellationOptions()
                {
                    StepDistance = 1.0f,
                    MaxCordDeviation = 0.5f,
                    MaxTanAngleDeviation = 0.1f,
                    SamplingStepSize = 0.01f
                },
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                minTextureSize = 64,
                maxTextureSize = 256,
                sampleCount = 4,
                gradientResolution = 128,
                pixelsPerUnit = 100f,
                scaleFactor = 1f,
                overrideNode = null,
                expandEdges = false
            };
        }
    }
}