Output.WriteLine($@"
namespace FScruiser.XF
{{
    public static partial class Secrets
    {{
		static Secrets()
		{{
			APPCENTER_KEY_DROID = ""{System.Environment.GetEnvironmentVariable("fscruiser_appcenter_key_droid") ?? ""}"";
			APPCENTER_KEY_UWP = ""{System.Environment.GetEnvironmentVariable("fscruiser_appcenter_key_uwp") ?? ""}"";
			APPCENTER_KEY_IOS = ""{System.Environment.GetEnvironmentVariable("fscruiser_appcenter_key_ios") ?? ""}"";
		}}
    }}
}}
");