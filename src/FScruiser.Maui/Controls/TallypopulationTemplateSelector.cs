namespace FScruiser.Maui.Controls;

public class TallyPopulationTemplateSelector : DataTemplateSelector
{
    public DataTemplate? ClickerTallyTemplate { get; set; }

    public DataTemplate? TallyTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        return TallyTemplate;

        //var tallyPop = (TallyPopulation)item;

        //return (tallyPop.IsClickerTally) ? ClickerTallyTemplate : TallyTemplate;
    }
}