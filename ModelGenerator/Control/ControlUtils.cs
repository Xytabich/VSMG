using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModelGenerator
{
    public static class ControlUtils
    {
        private static readonly RegexInputVerifier integerVerifier = new RegexInputVerifier(@"^[0-9]*$");
        private static readonly RegexInputVerifier signedIntegerVerifier = new RegexInputVerifier(@"^-?[0-9]*$");
        private static readonly RegexInputVerifier floatVerifier = new RegexInputVerifier(@"^-?[0-9]*([\.]([0-9]+([eE][-+]?\d*)?)?)?$");

        public static void InitIntegerField(this TextBox field)
        {
            integerVerifier.InitField(field);
        }

        public static void InitSignedIntegerField(this TextBox field)
        {
            signedIntegerVerifier.InitField(field);
        }

        public static void InitFloatField(this TextBox field)
        {
            floatVerifier.InitField(field);
        }

        public static int GetInteger(this TextBox field, int def = default)
        {
            int value;
            if(!int.TryParse(field.Text.Trim(), out value)) value = def;
            return value;
        }

        public static void SetInteger(this TextBox field, int value)
        {
            field.Text = value.ToString();
        }

        public static short GetShort(this TextBox field, short def = default)
        {
            short value;
            if(!short.TryParse(field.Text.Trim(), out value)) value = def;
            return value;
        }

        public static double GetDouble(this TextBox field, double def = default)
        {
            double value;
            if(!double.TryParse(field.Text.Trim(), NumberStyles.Float, NumberFormatInfo.InvariantInfo, out value)) value = def;
            return value;
        }

        public static void SetDouble(this TextBox field, double value)
        {
            field.Text = value.ToString("g", NumberFormatInfo.InvariantInfo);
        }

        public static float GetSingle(this TextBox field, float def = default)
        {
            float value;
            if(!float.TryParse(field.Text.Trim(), NumberStyles.Float, NumberFormatInfo.InvariantInfo, out value)) value = def;
            return value;
        }

        public static void SetSingle(this TextBox field, float value)
        {
            field.Text = value.ToString("g", NumberFormatInfo.InvariantInfo);
        }

        public class RegexInputVerifier
        {
            private Regex pattern;

            public RegexInputVerifier(string pattern)
            {
                this.pattern = new Regex(pattern);
            }

            public void InitField(TextBox field)
            {
                field.PreviewTextInput += IsValidText;
                DataObject.AddPastingHandler(field, IsValidPaste);
            }

            private void IsValidText(object sender, TextCompositionEventArgs e)
            {
                var box = sender as TextBox;
                e.Handled = !pattern.IsMatch(box.Text.Insert(box.CaretIndex, e.Text));
            }

            private void IsValidPaste(object sender, DataObjectPastingEventArgs e)
            {
                if(e.DataObject.GetDataPresent(typeof(string)))
                {
                    string text = (string)e.DataObject.GetData(typeof(string));
                    if(!pattern.IsMatch(text))
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
}