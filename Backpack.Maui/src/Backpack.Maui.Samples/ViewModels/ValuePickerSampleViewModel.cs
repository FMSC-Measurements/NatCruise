using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backpack.Maui.Samples.ViewModels
{
    public partial class ValuePickerSampleViewModel : ObservableObject
    {
        [ObservableProperty]
        private IList _pickerItems = new List<int>()
        { 1, 2, 3 };

        [ObservableProperty]
        private int _selectedItem;

        [ObservableProperty]
        private int _selectedValue;

        [ObservableProperty]
        private int _selectedIndex;

        [ObservableProperty]
        private string? _valueText;

    }
}
