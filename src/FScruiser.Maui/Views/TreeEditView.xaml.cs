using FScruiser.Maui.Controls;
using NatCruise.MVVM.ViewModels;

namespace FScruiser.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class TreeEditView : BasePage
{
    #region TreeNumber

    /// <summary>
    /// Identifies the <see cref="TreeNumber"/> bindable property.
    /// </summary>
    public static readonly BindableProperty TreeNumberProperty =
            BindableProperty.Create(nameof(TreeNumber),
              typeof(int?),
              typeof(TreeEditView),
              defaultValue: default(int?),
              defaultBindingMode: BindingMode.TwoWay,
              propertyChanged: (bindable, oldValue, newValue) => ((TreeEditView)bindable).OnTreeNumberChanged((int?)oldValue, (int?)newValue));

    /// <summary>
    /// Invoked after changes have been applied to the <see cref="TreeNumber"/> property.
    /// </summary>
    /// <param name="oldValue">The old value of the <see cref="TreeNumber"/> property.</param>
    /// <param name="newValue">The new value of the <see cref="TreeNumber"/> property.</param>
    protected virtual void OnTreeNumberChanged(int? oldValue, int? newValue)
    {
        _treeNumberEntry.Text = newValue?.ToString();
    }

    /// <summary>
    /// Gets or sets the <see cref="TreeNumber" /> property. This is a bindable property.
    /// </summary>
    public int? TreeNumber
    {
        get { return (int?)GetValue(TreeNumberProperty); }
        set { SetValue(TreeNumberProperty, value); }
    }

    #endregion TreeNumber

    protected TreeEditView()
	{
		InitializeComponent();

        _treeNumberEntry.Unfocused += _treeNumberEntry_Unfocused;
    }

    public TreeEditView(TreeEditViewModel viewModel) : this()
    {
        BindingContext = viewModel;
    }

    private void _treeNumberEntry_Unfocused(object? sender, FocusEventArgs e)
    {
        var value = _treeNumberEntry.Text;
        if (int.TryParse(value, out var intValue))
        {
            TreeNumber = intValue;
        }
    }
}