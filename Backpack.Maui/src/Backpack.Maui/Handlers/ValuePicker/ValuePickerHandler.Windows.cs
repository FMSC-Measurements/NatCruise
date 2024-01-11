using Backpack.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backpack.Maui.Handlers.ValuePicker
{
    public partial class ValuePickerHandler : ViewHandler<IValuePicker, ComboBox>
    {
        protected override ComboBox CreatePlatformView() =>
        throw new NotImplementedException();

        protected override void ConnectHandler(ComboBox platformView)
        {
            throw new NotImplementedException();
        }

        protected override void DisconnectHandler(ComboBox platformView)
        {
            throw new NotImplementedException();
        }

        public static void MapBackground(ValuePickerHandler handler, IPicker picker)
        {
            throw new NotImplementedException();
        }

        internal static void MapItems(ValuePickerHandler handler, IPicker picker) =>
            throw new NotImplementedException();

        public static void MapTitle(ValuePickerHandler handler, IPicker picker) =>
            throw new NotImplementedException();
        public static void MapTitleColor(ValuePickerHandler handler, IPicker picker) =>
            throw new NotImplementedException();
        public static void MapSelectedIndex(ValuePickerHandler handler, IPicker picker) =>
            throw new NotImplementedException();
        public static void MapCharacterSpacing(ValuePickerHandler handler, IPicker picker) =>
            throw new NotImplementedException();
        public static void MapFont(ValuePickerHandler handler, IPicker picker) =>
            throw new NotImplementedException();
        public static void MapTextColor(ValuePickerHandler handler, IPicker picker) =>
            throw new NotImplementedException();
        public static void MapHorizontalTextAlignment(ValuePickerHandler handler, IPicker picker) =>
            throw new NotImplementedException();
        public static void MapVerticalTextAlignment(ValuePickerHandler handler, IPicker picker) =>
            throw new NotImplementedException();
    }
}
