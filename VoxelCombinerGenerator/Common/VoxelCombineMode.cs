using System.ComponentModel.DataAnnotations;

namespace VoxelCombinerGenerator
{
    public enum VoxelCombineMode
    {
        [Display(Name = "Add", Description = "Adds new voxels without replacing existing ones")]
        DstOverlap,
        [Display(Name = "Overlap", Description = "Adds new voxels replacing existing ones")]
        SrcOverlap,
        [Display(Name = "Intersect", Description = "Leaves only overlapping voxels but does not replace them with new ones")]
        SrcIntersect,
        [Display(Name = "Intersect Replace", Description = "Leaves only intersecting voxels and also replaces them with new ones")]
        DstIntersect,
        [Display(Name = "Subtract", Description = "Removes voxels by shape")]
        SrcSubtract,
        [Display(Name = "Swap Subtract", Description = "Same as subtraction but the shapes are swapped")]
        DstSubtract,
        [Display(Name = "Replace", Description = "Replaces intersecting voxels with new ones")]
        SrcReplaceIntersect,
        [Display(Name = "Swap Replace", Description = "Same as replacement but the shapes are swapped")]
        DstReplaceIntersect,
        [Display(Name = "Difference", Description = "Adds new voxels but removes intersecting voxels")]
        Difference
    }
}