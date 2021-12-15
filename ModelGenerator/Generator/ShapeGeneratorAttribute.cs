using System;

namespace ModelGenerator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ShapeGeneratorAttribute : Attribute
    {
        public readonly string name;

        public ShapeGeneratorAttribute(string name)
        {
            this.name = name;
        }
    }
}