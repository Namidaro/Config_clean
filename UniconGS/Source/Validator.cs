using System.Windows.Controls;

namespace UniconGS.Source
{
    public static class Validator
    {
        public static bool ValidateTextBox(TextBox tb, int minVlue, int maxValue)
        {
            int result;
            bool ret = int.TryParse(tb.Text, out result);
            if (string.IsNullOrEmpty(tb.Text) || int.Parse(tb.Text) < minVlue || int.Parse(tb.Text) > maxValue)
            {
                ret = false;
                return ret;
            }
            return ret;
        }
    }
}
