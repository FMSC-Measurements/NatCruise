using FScruiser.XF.ViewModels;
using Prism.Navigation;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage, IMasterDetailPageOptions
    {
        public bool IsPresentedAfterNavigation => Device.Idiom != TargetIdiom.Phone;

        public MainPage()
        {
            try
            {
                InitializeComponent();

                MenuItemsListView.ItemSelected += ListView_ItemSelected;

                MessagingCenter.Subscribe<object, string>(this, Messages.CRUISE_FILE_OPENED, (sender, path) =>
                {
                    IsPresented = false;
                });

                MessagingCenter.Subscribe<string>(this, Messages.CUTTING_UNIT_SELECTED, (o) =>
                {
                    IsPresented = true;
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is NavigationListItem item && item != null
              && Master.BindingContext is MainViewModel vm && vm != null)
            {
                var task = vm.NavigateToAsync(item);
            }

            MenuItemsListView.SelectedItem = null;
        }

        private void _selectFile_Tapped(object sender, EventArgs ea)
        {
            IsPresented = false;
        }

        //private async System.Threading.Tasks.Task ShowPageFromNavigationListItemAsync(NavigationListItem item)
        //{
        //    if (item.CanShow == false) { return; }

        //    var navigation = Detail.Navigation;
        //    if (item.ResetsNavigation)
        //    {
        //        await navigation.PopToRootAsync();
        //    }
        //    else
        //    {
        //        var page = item.MakePage();
        //        page.Title = item.Title;

        //        page.SetValue(NavigationPage.HasBackButtonProperty, false);

        //        await Detail.Navigation.PushAsync(page);
        //    }

        //    IsPresented = false;
        //}
    }
}