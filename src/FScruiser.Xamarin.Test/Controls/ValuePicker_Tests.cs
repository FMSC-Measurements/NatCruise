using FluentAssertions;
using NatCruise.MVVM;
using NatCruise.Test;
using OpenTK.Platform.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.XF.Controls
{
    public class ValuePicker_Tests : TestBase
    {
        public ValuePicker_Tests(ITestOutputHelper output) : base(output)
        {
            // initializes mock platform services 
            MockForms.Init();
        }

        class ItemData
        {
            public string A { get; set; }
            public string B { get; set; }

            public override string ToString()
            {
                return A + B;
            }
        }

        class MockViewModel : ViewModelBase
        {
            private IList _data;
            private object _selectedItem;
            private object _selectedValue;
            private string _text;

            public IList Data { get => _data; set => SetProperty(ref _data, value); }
            public object SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
            public object SelectedValue { get => _selectedValue; set => SetProperty(ref _selectedValue, value); }
            public string Text { get => _text; set => SetProperty(ref _text, value); }
        }

        IList MakeItems_ItemData()
        {
            return new List<ItemData>()
            {
                new ItemData() { A = "a1", B = "b1" },
                new ItemData() { A = "a2", B = "b2" },
                new ItemData() { A = "a3", B = "b3" },
            };
        }

        IList Make_StringData(int count)
        {
            return Enumerable.Range(1, count)
                .Select(x => x.ToString())
                .ToList();
        }




        [Fact]
        public void Set_ItemsSource()
        {
            var mockVM = new MockViewModel
            {
                Data = MakeItems_ItemData(),
            };

            var picker = new ValuePicker();
            picker.SetBinding(ValuePicker.ItemsSourceProperty, new Binding { Path = "Data" });

            picker.BindingContext = mockVM;

            picker.Items.Should().HaveSameCount(mockVM.Data);
            picker.Items.Should().BeEquivalentTo(mockVM.Data);
            picker.ItemsSource.Should().BeSameAs(mockVM.Data);
            picker.SelectedIndex.Should().Be(-1);
            picker.SelectedItem.Should().BeNull();
            picker.SelectedValue.Should().BeNull();
            picker.Text.Should().BeNull();
        }

        [Fact]
        public void Initialize_With_SelectedItem()
        {
            var mockVM = new MockViewModel
            {
                Data = MakeItems_ItemData(),
            };
            mockVM.SelectedItem = mockVM.Data[1];

            var picker = new ValuePicker();
            picker.SetBinding(ValuePicker.ItemsSourceProperty, new Binding { Path = "Data" });
            picker.SetBinding(ValuePicker.SelectedItemProperty, new Binding { Path = "SelectedItem" });

            picker.BindingContext = mockVM;

            picker.Items.Should().HaveSameCount(mockVM.Data);
            picker.Items.Should().BeEquivalentTo(mockVM.Data);
            picker.ItemsSource.Should().BeSameAs(mockVM.Data);
            picker.SelectedIndex.Should().Be(1);
            picker.SelectedItem.Should().BeSameAs(mockVM.SelectedItem);
            picker.SelectedValue.Should().BeSameAs(mockVM.SelectedItem);
            picker.Text.Should().Be(mockVM.SelectedItem.ToString());
        }

        [Fact(DisplayName = "Test SelectedValue with a complex type where slectedValue is in the Data Collection")]
        public void Initialize_SelectedValue_WithComplexType_RefEqls()
        {
            var mockVM = new MockViewModel
            {
                Data = MakeItems_ItemData(),
            };
            mockVM.SelectedValue = mockVM.Data[1];

            var picker = new ValuePicker();
            picker.SetBinding(ValuePicker.ItemsSourceProperty, new Binding { Path = "Data" });
            picker.SetBinding(ValuePicker.SelectedValueProperty, new Binding { Path = "SelectedValue" });

            picker.BindingContext = mockVM;

            picker.Items.Should().HaveSameCount(mockVM.Data);
            picker.Items.Should().BeEquivalentTo(mockVM.Data);
            picker.ItemsSource.Should().BeSameAs(mockVM.Data);
            picker.SelectedIndex.Should().Be(1);
            picker.SelectedItem.Should().BeSameAs(mockVM.SelectedValue);
            picker.SelectedValue.Should().BeSameAs(mockVM.SelectedValue);
            picker.Text.Should().Be(mockVM.SelectedValue.ToString());
        }

        [Fact(DisplayName = "Test SelectedValue with a complex type where slectedValue is in the Data Collection")]
        public void Initialize_SelectedValue_ComplexType_ValuePath()
        {
            var mockVM = new MockViewModel
            {
                Data = MakeItems_ItemData(),
            };
            var item = (ItemData)mockVM.Data[1];
            mockVM.SelectedValue = item.A;

            var picker = new ValuePicker();
            picker.SelectedValuePath = "A";

            picker.SetBinding(ValuePicker.ItemsSourceProperty, new Binding { Path = "Data" });
            picker.SetBinding(ValuePicker.SelectedValueProperty, new Binding { Path = "SelectedValue" });

            picker.BindingContext = mockVM;

            picker.Items.Should().HaveSameCount(mockVM.Data);
            picker.Items.Should().BeEquivalentTo(mockVM.Data);
            picker.ItemsSource.Should().BeSameAs(mockVM.Data);
            picker.SelectedIndex.Should().Be(1);
            picker.SelectedItem.Should().BeSameAs(item);
            picker.SelectedValue.Should().BeSameAs(mockVM.SelectedValue);
            picker.Text.Should().Be(item.ToString());
        }

        [Fact]
        [Trait("Description", "Test Selected Value with a value type")]
        public void Initialize_SelectedValue_WithStringData()
        {
            var mockVM = new MockViewModel
            {
                Data = Make_StringData(3),
            };
            mockVM.SelectedValue = "2";

            var picker = new ValuePicker();
            picker.SetBinding(ValuePicker.ItemsSourceProperty, new Binding { Path = "Data" });
            picker.SetBinding(ValuePicker.SelectedValueProperty, new Binding { Path = "SelectedValue" });

            picker.BindingContext = mockVM;

            picker.Items.Should().HaveSameCount(mockVM.Data);
            picker.Items.Should().BeEquivalentTo(mockVM.Data);
            picker.ItemsSource.Should().BeSameAs(mockVM.Data);
            picker.SelectedIndex.Should().Be(1);
            picker.SelectedItem.Should().BeSameAs(mockVM.Data[1]);
            picker.SelectedValue.Should().BeSameAs(mockVM.SelectedValue);
            picker.Text.Should().Be(mockVM.SelectedValue.ToString());
        }


        [Fact]
        [Trait("Description", "")]
        public void Change_SelectedValue_WithStringData()
        {
            Output.WriteLine(BindableProperty.UnsetValue.ToString() + BindableProperty.UnsetValue.GetHashCode().ToString());
            var mockVM = new MockViewModel
            {
                Data = Make_StringData(3),
            };


            var picker = new ValuePicker();
            picker.SetBinding(ValuePicker.ItemsSourceProperty, new Binding { Path = "Data" });
            picker.SetBinding(ValuePicker.SelectedValueProperty, new Binding { Path = "SelectedValue" });

            picker.BindingContext = mockVM;

            picker.Items.Should().HaveSameCount(mockVM.Data);
            picker.Items.Should().BeEquivalentTo(mockVM.Data);
            picker.ItemsSource.Should().BeSameAs(mockVM.Data);
            picker.SelectedIndex.Should().Be(-1);
            picker.SelectedItem.Should().BeNull();
            picker.SelectedValue.Should().BeNull();
            picker.Text.Should().BeNull();

            int valueCounter = 0;
            int itemCounter = 0;
            picker.PropertyChanged += (sender, args) => {
                if (args.PropertyName == nameof(picker.SelectedValue)) valueCounter++;
                if (args.PropertyName == nameof(picker.SelectedItem)) itemCounter++;
            };

            mockVM.SelectedValue = "2";

            valueCounter.Should().Be(1);
            itemCounter.Should().Be(1);

            var expectedItemIndex = 1;
            picker.SelectedIndex.Should().Be(expectedItemIndex);
            picker.SelectedItem.Should().BeSameAs(mockVM.Data[expectedItemIndex]);
            picker.SelectedValue.Should().BeSameAs(mockVM.SelectedValue);
            picker.Text.Should().Be(mockVM.SelectedValue.ToString());

            mockVM.SelectedValue = "1";

            valueCounter.Should().Be(2);
            itemCounter.Should().Be(2);

            expectedItemIndex = 0;
            picker.SelectedIndex.Should().Be(expectedItemIndex);
            picker.SelectedItem.Should().BeSameAs(mockVM.Data[expectedItemIndex]);
            picker.SelectedValue.Should().BeSameAs(mockVM.SelectedValue);
            picker.Text.Should().Be(mockVM.SelectedValue.ToString());

            mockVM.SelectedValue = null;
            valueCounter.Should().Be(3);
            itemCounter.Should().Be(3);

            expectedItemIndex = -1;
            picker.SelectedIndex.Should().Be(expectedItemIndex);
            picker.SelectedItem.Should().BeNull();
            picker.SelectedValue.Should().BeSameAs(mockVM.SelectedValue);
            picker.Text.Should().BeNull();
        }

        [Fact]
        [Trait("Description", "")]
        public void Change_ItemSource_WithInitializedSelectedItem()
        {

            var mockVM = new MockViewModel
            {
                SelectedValue = "1",
            };


            var picker = new ValuePicker();
            picker.SetBinding(ValuePicker.ItemsSourceProperty, new Binding { Path = "Data" });
            picker.SetBinding(ValuePicker.SelectedValueProperty, new Binding { Path = "SelectedValue" });

            picker.BindingContext = mockVM;

            picker.Items.Should().BeEmpty();
            picker.ItemsSource.Should().BeNull();
            picker.SelectedIndex.Should().Be(-1);
            picker.SelectedItem.Should().BeNull();
            picker.SelectedValue.Should().Be(mockVM.SelectedValue);
            picker.Text.Should().BeNull();

            int valueCounter = 0;
            int itemCounter = 0;
            picker.PropertyChanged += (sender, args) => {
                if (args.PropertyName == nameof(picker.SelectedValue)) valueCounter++;
                if (args.PropertyName == nameof(picker.SelectedItem)) itemCounter++;
            };

            mockVM.Data = Make_StringData(3);

            valueCounter.Should().Be(0);
            itemCounter.Should().Be(1);
            picker.Items.Should().HaveSameCount(mockVM.Data);
            picker.Items.Should().BeEquivalentTo(mockVM.Data);
            picker.ItemsSource.Should().BeSameAs(mockVM.Data);

            var expectedItemIndex = 0;
            picker.SelectedIndex.Should().Be(expectedItemIndex);
            picker.SelectedItem.Should().BeSameAs(mockVM.Data[expectedItemIndex]);
            picker.SelectedValue.Should().BeSameAs(mockVM.SelectedValue);
            picker.Text.Should().Be(mockVM.SelectedValue.ToString());

            mockVM.SelectedValue = "2";
            valueCounter.Should().Be(1);
            itemCounter.Should().Be(2);

            expectedItemIndex = 1;
            picker.SelectedIndex.Should().Be(expectedItemIndex);
            picker.SelectedItem.Should().BeSameAs(mockVM.Data[expectedItemIndex]);
            picker.SelectedValue.Should().BeSameAs(mockVM.SelectedValue);
            picker.Text.Should().Be(mockVM.SelectedValue.ToString());


            mockVM.SelectedValue = null;
            valueCounter.Should().Be(2);
            itemCounter.Should().Be(3);

            expectedItemIndex = -1;
            picker.SelectedIndex.Should().Be(expectedItemIndex);
            picker.SelectedItem.Should().BeNull();
            picker.SelectedValue.Should().BeNull();
            picker.Text.Should().BeNull();

        }
    }
}
