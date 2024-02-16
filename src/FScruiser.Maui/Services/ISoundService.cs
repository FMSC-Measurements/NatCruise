namespace FScruiser.Maui.Services;

public interface ISoundService : IDisposable
{
    Task SignalMeasureTreeAsync();

    Task SignalInsuranceTreeAsync();

    Task SignalTallyAsync(bool force = false);

    Task SignalInvalidActionAsync();

    void PlayClickSound();
}