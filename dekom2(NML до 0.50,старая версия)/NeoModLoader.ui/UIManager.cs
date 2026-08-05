using NeoModLoader.api;
using NeoModLoader.General;
using NeoModLoader.General.UI.Window;
using NeoModLoader.utils;
using UnityEngine;

namespace NeoModLoader.ui;

internal static class UIManager
{
	public static void init()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		SingleAutoLayoutWindow<InformationWindow>.CreateWindow("Information", "Information Title");
		AbstractListWindow<ModListWindow, IMod>.CreateAndInit("NeoModList");
		AbstractListWindow<WorkshopModListWindow, ModDeclare>.CreateAndInit("WorkshopMods");
		AbstractWindow<ModUploadWindow>.CreateAndInit("ModUpload");
		AbstractWindow<ModUploadingProgressWindow>.CreateAndInit("ModUploadingProgress");
		AbstractWindow<ModUploadAuthenticationWindow>.CreateAndInit("ModUploadAuthentication");
		AbstractWindow<ModConfigureWindow>.CreateAndInit("ModConfigure");
		PowerButtonCreator.AddButtonToTab(PowerButtonCreator.CreateWindowButton("NML_ModsList", "NeoModList", InternalResourcesGetter.GetIcon()), PowerButtonCreator.GetTab("Tab_Main"), new Vector2(403.2f, -18f));
	}
}
