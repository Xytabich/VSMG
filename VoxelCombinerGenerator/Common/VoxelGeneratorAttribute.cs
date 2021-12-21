using System;

namespace VoxelCombinerGenerator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VoxelGeneratorAttribute : Attribute
    {
        public readonly string name;
        public readonly Type saveDataType;
        public readonly CombinerHideControls hideControls;

        public VoxelGeneratorAttribute(string name)
        {
            this.name = name;
            this.saveDataType = null;
            this.hideControls = CombinerHideControls.None;
        }

        public VoxelGeneratorAttribute(string name, Type saveDataType)
        {
            this.name = name;
            this.saveDataType = saveDataType;
            this.hideControls = CombinerHideControls.None;
        }

        public VoxelGeneratorAttribute(string name, Type saveDataType, CombinerHideControls hideControls)
        {
            this.name = name;
            this.saveDataType = saveDataType;
            this.hideControls = hideControls;
        }
    }
}