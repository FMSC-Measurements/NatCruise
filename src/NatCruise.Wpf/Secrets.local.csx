Output.WriteLine($@"
namespace NatCruise.Wpf
{{
    public static partial class Secrets
    {{
		static Secrets()
		{{
			APPCENTER_KEY_WINDOWS = ""{System.Environment.GetEnvironmentVariable("natcruise_appcenterr_key_windows") ?? ""}"";
		}}
    }}
}}
");