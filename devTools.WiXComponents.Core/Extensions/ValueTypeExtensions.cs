// ReSharper disable once CheckNamespace
namespace devTools.WiXComponents.Extensions;

public static class ValueTypeExtensions
{
	public static string ToYesNo(this bool thisValue)
	{
		return thisValue
					? "yes"
					: "no";
	}

	public static string ToYesNo(this bool? thisValue)
	{
		return thisValue == true
					? "yes"
					: "no";
	}
}