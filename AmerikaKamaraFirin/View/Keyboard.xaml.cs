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
        private bool isClosing = false;

        public Keyboard(string str)
        {
            InitializeComponent();
            KeyText.Text = str;
            KeyText.CaretIndex = str.Length;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(100); // dokunmatik cihazlar için kısa gecikme
            KeyText.Focus();
            KeyText.SelectAll();
        }

        private void Window_TouchDown(object sender, TouchEventArgs e)
        {
            this.Focus();
            KeyText.Focus();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void Key_Click(object sender, RoutedEventArgs e)
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
            this.DialogResult = true;
            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            isClosing = true;
            base.OnClosing(e);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (!isClosing && this.IsLoaded && this.IsVisible)
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    if (!isClosing && this.IsLoaded && this.IsVisible)
                        this.Close();
                });
            }
        }
    }
}
