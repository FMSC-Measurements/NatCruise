Output.WriteLine($@"
namespace NatCruise.Wpf
{{
    public static partial class Secrets
    {{
		static Secrets()
		{{
			APPCENTER_KEY_WINDOWS = ""{System.Environment.GetEnvironmentVariable("cruisemanager_appcenter_key_windows") ?? ""}"";
		}}
    }}
}}
");