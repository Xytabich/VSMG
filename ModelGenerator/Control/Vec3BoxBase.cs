using System.Windows.Controls;

namespace ModelGenerator
{
    public abstract class Vec3BoxBase : Control
    {
        protected TextBox x = null;
        protected TextBox y = null;
        protected TextBox z = null;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            x = GetTemplateChild("x") as TextBox;
            y = GetTemplateChild("y") as TextBox;
            z = GetTemplateChild("z") as TextBox;
            InitInputs();
            x.TextChanged += OnXChanged;
            y.TextChanged += OnYChanged;
            z.TextChanged += OnZChanged;
        }

        protected abstract void InitInputs();

        protected abstract void OnXChanged(object sender, TextChangedEventArgs e);
        protected abstract void OnYChanged(object sender, TextChangedEventArgs e);
        protected abstract void OnZChanged(object sender, TextChangedEventArgs e);
    }

    public abstract class Vec3BoxBase<T> : Vec3BoxBase where T : new()
    {
        protected T value = new T();

        public T GetValue()
        {
            return value;
        }

        public abstract void SetValue(T value);
    }
}