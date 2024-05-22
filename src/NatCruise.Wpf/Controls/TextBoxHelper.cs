using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NatCruise.Wpf.Controls
{
    public static class TextBoxHelper
    {
        public static readonly DependencyProperty RegexMaskProperty = DependencyProperty.RegisterAttached(
            "RegexMask",
            typeof(string),
            typeof(TextBoxHelper),
            new UIPropertyMetadata(OnRegexMaskChanged)
            );

        public static string GetRegexMask(DependencyObject obj)
        {
            return (string)obj.GetValue(RegexMaskProperty);
        }

        public static void SetRegexMask(DependencyObject obj, string value)
        {
            obj.SetValue(RegexMaskProperty, value);
        }

        public static void OnRegexMaskChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var textbox = obj as TextBox;
            if (textbox == null) return;

            textbox.PreviewTextInput += TextBox_PreviewTextInput;
            DataObject.AddPastingHandler(textbox, TextBox_Pasting);
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // note: previewTextInput event is not called if Space, backspace, Delete are pressed


            var textbox = sender as TextBox;

            var mask = GetRegexMask(textbox);
            if (string.IsNullOrWhiteSpace(mask)) return;
            e.Handled = true;

            var text = textbox.Text;
            var position = textbox.CaretIndex;
            //var nextPosition = position;
            var input = e.Text;
            var inputLen = input.Length;

            var selectionLength = textbox.SelectionLength;
            if (selectionLength > 0)
            {
                var selectionStart = textbox.SelectionStart;
                var selectionEnd = selectionStart + selectionLength;
                if (selectionLength == text.Length)
                {
                    text = input;
                }
                else if (selectionStart == 0)
                {
                    text = input + text.Substring(selectionEnd);
                }
                else
                {
                    text = text.Substring(0, selectionStart) + input + text.Substring(selectionEnd);
                }
            }
            else if (Keyboard.IsKeyToggled(Key.Insert))
            {
                // calculate length of text after adding edits
                var textLength = Math.Max(position + inputLen, text.Length);

                // copy existing text over to char array 
                var chars = new char[textLength];
                text.CopyTo(0, chars, 0, text.Length);

                // copy over inserted text to char arry
                input.CopyTo(0, chars, position, inputLen);

                text = new String(chars);
            }
            else
            {
                text = text.Insert(position, input);
            }

            var maxLength = textbox.MaxLength;
            if(maxLength > 0 && text.Length > maxLength) { return; }

            // test new text value against our Regex mask
            var match = Regex.Match(text, mask, RegexOptions.None, TimeSpan.FromMilliseconds(100));
            // if there is a match and all the text is part of the match
            // it might be possible to have multiple matches but we aren't allowing that
            if (match != null && match.Length == text.Length)
            {
                textbox.SetValue(TextBox.TextProperty, text);

                // advance cursor
                var nextPosition = position + inputLen;
                textbox.CaretIndex = nextPosition;

            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private static void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            var textbox = sender as TextBox;

            // if is string and not drag and drop
            if (e.DataObject.GetDataPresent(typeof(string)) && !e.IsDragDrop)
            {
                var pastedText = (string)e.DataObject.GetData(typeof(string));

                var text = textbox.Text;
                var position = textbox.CaretIndex;
                var inputLen = pastedText.Length;

                text = text.Insert(position, pastedText);

                var mask = GetRegexMask(textbox);
                if (mask == null) return;
                // test new text value against our Regex mask
                var match = Regex.Match(text, mask, RegexOptions.None, TimeSpan.FromMilliseconds(100));
                // if there is a match and all the text is part of the match
                // it might be possible to have multiple matches but we aren't allowing that
                if (match != null && match.Length == text.Length)
                {
                    textbox.SetValue(TextBox.TextProperty, text);

                    // advance cursor
                    var nextPosition = position + inputLen;
                    textbox.CaretIndex = nextPosition;

                    textbox.Focus();

                }
                else
                {
                    System.Media.SystemSounds.Beep.Play();
                }

                e.Handled = true;
            }
            else
            {
                // cancel drag and drop event
                e.CancelCommand();
            }
            
        }
    }
}