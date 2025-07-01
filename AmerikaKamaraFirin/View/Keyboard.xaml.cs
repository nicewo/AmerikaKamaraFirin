using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AmerikaKamaraFirin.View
{
    public partial class Keyboard : Window
    {
        public string GirilenMetin => KeyText.Text;
        bool dokunmatik = false;

        public Keyboard(string str)
        {
            InitializeComponent();
            KeyText.Text = str;
            KeyText.SelectAll();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (!dokunmatik)
            {
                this.Close();
            }
        }
        private void Close_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dokunmatik = true;
            this.Close();
        }

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            if (!dokunmatik)
            {
                if (sender is Button btn && btn.Content != null)
                {
                    var key = btn.Content.ToString();
                    int caret = KeyText.CaretIndex;

                    if (!string.IsNullOrEmpty(KeyText.SelectedText))
                    {
                        int selStart = KeyText.SelectionStart;
                        KeyText.Text = KeyText.Text.Remove(selStart, KeyText.SelectionLength);
                        if (caret > 0) key = key.ToLower();
                        KeyText.Text = KeyText.Text.Insert(selStart, key);
                        KeyText.CaretIndex = selStart + key.Length;
                    }
                    else
                    {
                        if (caret > 0) key = key.ToLower();
                        KeyText.Text = KeyText.Text.Insert(caret, key);
                        KeyText.CaretIndex = caret + key.Length;
                    }
                }
            }
        }
        private void Key_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dokunmatik = true;
            if (sender is Button btn && btn.Content != null)
            {
                var key = btn.Content.ToString();
                int caret = KeyText.CaretIndex;

                if (!string.IsNullOrEmpty(KeyText.SelectedText))
                {
                    int selStart = KeyText.SelectionStart;
                    KeyText.Text = KeyText.Text.Remove(selStart, KeyText.SelectionLength);
                    if (caret > 0) key = key.ToLower();
                    KeyText.Text = KeyText.Text.Insert(selStart, key);
                    KeyText.CaretIndex = selStart + key.Length;
                }
                else
                {
                    if (caret > 0) key = key.ToLower();
                    KeyText.Text = KeyText.Text.Insert(caret, key);
                    KeyText.CaretIndex = caret + key.Length;
                }
            }
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (!dokunmatik)
            {
                int caret = KeyText.CaretIndex;
                if (caret > 0)
                {
                    KeyText.Text = KeyText.Text.Remove(caret - 1, 1);
                    KeyText.CaretIndex = caret - 1;
                }
            }
        }
        private void Backspace_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dokunmatik = true;
            int caret = KeyText.CaretIndex;
            if (caret > 0)
            {
                KeyText.Text = KeyText.Text.Remove(caret - 1, 1);
                KeyText.CaretIndex = caret - 1;
            }

        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (!dokunmatik)
            {
                this.DialogResult = true;
                this.Close();
            }
        }
        private void Enter_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dokunmatik = true;
            this.DialogResult = true;
            this.Close();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                KeyText.Focus();
            }), System.Windows.Threading.DispatcherPriority.Input);

        }





    }
}
