using System;

namespace VoxelCombinerGenerator
{
    /// <summary>
    /// What additional controls will not be shown for the generator
    /// </summary>
    [Flags]
    public enum CombinerHideControls
    {
        None = 0,
        Material = 1
    }
}