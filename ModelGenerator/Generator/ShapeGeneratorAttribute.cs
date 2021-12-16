using System;

namespace ModelGenerator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ShapeGeneratorAttribute : Attribute
    {
        public readonly string name;
        public readonly Type presetType;

        public ShapeGeneratorAttribute(string name)
        {
            this.name = name;
            this.presetType = null;
        }

        public ShapeGeneratorAttribute(string name, Type presetType)
        {
            this.name = name;
            this.presetType = presetType;
        }
    }
}