using System.Collections.Generic;
using NeoModLoader.services;
using UnityEngine;

namespace NeoModLoader.General.UI.Tab;

public static class PowersTabExtension
{
	private static Dictionary<string, WrappedPowersTab> _wrapped_powers_tabs = new Dictionary<string, WrappedPowersTab>();

	public static void SetLayout(this PowersTab pTab, List<string> pGroupIds)
	{
		WrappedPowersTab wrappedPowersTab = _getWrappedPowersTab(pTab);
		if (!wrappedPowersTab.Modifiable)
		{
			LogService.LogWarning(((Object)pTab).name + "'s layout cannot be changed");
			LogService.LogStackTraceAsWarning();
			return;
		}
		wrappedPowersTab.ResetGroups();
		foreach (string pGroupId in pGroupIds)
		{
			wrappedPowersTab.AddGroup(pGroupId);
		}
		wrappedPowersTab.Modifiable = false;
	}

	public static void AddPowerButton(this PowersTab pTab, string pGroupId, PowerButton pPowerButton)
	{
		WrappedPowersTab wrappedPowersTab = _getWrappedPowersTab(pTab);
		if (!wrappedPowersTab.HasGroup(pGroupId))
		{
			LogService.LogWarning(((Object)pTab).name + "'s layout does not contain group \"" + pGroupId + "\"");
			LogService.LogStackTraceAsWarning();
		}
		else
		{
			wrappedPowersTab.AddPowerButton(pGroupId, pPowerButton);
		}
	}

	public static void PutElement(this PowersTab pTab, string pGroupId, RectTransform pObjRect, Vector2 pPositionInGroup, bool pPlacehold = true)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		WrappedPowersTab wrappedPowersTab = _getWrappedPowersTab(pTab);
		if (!wrappedPowersTab.HasGroup(pGroupId))
		{
			LogService.LogWarning(((Object)pTab).name + "'s layout does not contain group \"" + pGroupId + "\"");
			LogService.LogStackTraceAsWarning();
		}
		else
		{
			wrappedPowersTab.AddCustomRect(pGroupId, pObjRect, pPositionInGroup, pPlacehold);
		}
	}

	public static void UpdateLayout(this PowersTab pTab)
	{
		_getWrappedPowersTab(pTab).UpdateLayout();
	}

	private static WrappedPowersTab _getWrappedPowersTab(PowersTab pTab)
	{
		if (!_wrapped_powers_tabs.TryGetValue(((Object)pTab).name, out var value))
		{
			value = new WrappedPowersTab(pTab);
			_wrapped_powers_tabs.Add(((Object)pTab).name, value);
		}
		return value;
	}
}
