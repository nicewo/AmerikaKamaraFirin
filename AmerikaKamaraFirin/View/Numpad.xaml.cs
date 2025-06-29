using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AmerikaKamaraFirin.View
{
    public partial class Numpad : Window
    {
        public string GirilenMetin => KeyText.Text;
        int miniValue, maxiValue;

        public Numpad(string str, int minValue = 0, int maxValue = 100)
        {
            InitializeComponent();
            miniValue = minValue;
            maxiValue = maxValue;
            lbl_min_value.Content = minValue.ToString();
            lbl_max_value.Content = maxValue.ToString();
            KeyText.Text = str;
            KeyText.SelectAll();
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Content != null)
            {
                var key = btn.Content.ToString();
                if (!string.IsNullOrEmpty(KeyText.SelectedText))
                {
                    int selStart = KeyText.SelectionStart;
                    KeyText.Text = KeyText.Text.Remove(selStart, KeyText.SelectionLength);
                    KeyText.Text = KeyText.Text.Insert(selStart, key);
                    KeyText.CaretIndex = selStart + key.Length;
                }
                else
                {
                    int caret = KeyText.CaretIndex;
                    KeyText.Text = KeyText.Text.Insert(caret, key);
                    KeyText.CaretIndex = caret + key.Length;
                }
            }
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            int caret = KeyText.CaretIndex;
            if (caret > 0)
            {
                KeyText.Text = KeyText.Text.Remove(caret - 1, 1);
                KeyText.CaretIndex = caret - 1;
            }
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            lbl_Error.Content = string.Empty;
            if (int.TryParse(KeyText.Text, out int value))
            {
                if (value < miniValue || value > maxiValue)
                {
                    lbl_Error.Content = AmerikaKamaraFirin.Resources.OutOfRange;
                    return;
                }

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                lbl_Error.Content = AmerikaKamaraFirin.Resources.OutOfRange;
            }
        }
    }
}
