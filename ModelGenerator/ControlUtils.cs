using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModelGenerator
{
    public static class ControlUtils
    {
        public static void InitIntegerField(this TextBox field)
        {
            field.PreviewTextInput += IsIntegerText;
            DataObject.AddPastingHandler(field, IsIntegerPaste);
        }

        public static void InitFloatField(this TextBox field)
        {
            field.PreviewTextInput += IsFloatText;
            DataObject.AddPastingHandler(field, IsFloatPaste);
        }

        public static int GetInteger(this TextBox field, int def = default)
        {
            int value;
            if(!int.TryParse(field.Text.Trim(), out value)) value = def;
            return value;
        }

        public static double GetDouble(this TextBox field, double def = default)
        {
            double value;
            if(!double.TryParse(field.Text.Trim(), out value)) value = def;
            return value;
        }

        private static readonly Regex integerPattern = new Regex(@"^[0-9]*$");
        private static void IsIntegerText(object sender, TextCompositionEventArgs e)
        {
            var box = sender as TextBox;
            e.Handled = !integerPattern.IsMatch(box.Text.Insert(box.CaretIndex, e.Text));
        }
        private static void IsIntegerPaste(object sender, DataObjectPastingEventArgs e)
        {
            if(e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if(!integerPattern.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static readonly Regex floatPattern = new Regex(@"^-?[0-9]*[\.]?[0-9]*$");
        private static void IsFloatText(object sender, TextCompositionEventArgs e)
        {
            var box = sender as TextBox;
            e.Handled = !floatPattern.IsMatch(box.Text.Insert(box.CaretIndex, e.Text));
        }
        private static void IsFloatPaste(object sender, DataObjectPastingEventArgs e)
        {
            if(e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if(!floatPattern.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}