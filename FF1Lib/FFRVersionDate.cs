using System.Globalization;
using System.Reflection;

namespace FF1Lib
{
	public static partial class FFRVersion
	{
		const string BuildVersionMetadataPrefix = "+build";
		const string dateFormat = "yyyy-MM-ddTHH:mm:ss:fffZ";
		public static DateTime GetLinkerTime()
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

			if (attribute?.InformationalVersion != null)
			{
				var value = attribute.InformationalVersion;
				var index = value.IndexOf(BuildVersionMetadataPrefix);
				if (index > 0)
				{
					value = value.Substring(index + BuildVersionMetadataPrefix.Length);
					return DateTime.ParseExact(value, dateFormat, CultureInfo.InvariantCulture);
				}
			}
			return DateTime.UnixEpoch;
		}
	}
}
