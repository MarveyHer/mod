using System;
using NeoModLoader.api;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;

namespace NeoModLoader.ui.prefabs;

public class ModInfoPanel : APrefab<ModInfoPanel>
{
	internal void Setup(ModDeclare pModDeclaration)
	{
		ModState modState = WorldBoxMod.AllRecognizedMods[pModDeclaration];
		if (modState == ModState.LOADED)
		{
			IMod mod = WorldBoxMod.LoadedMods.Find((IMod x) => x.GetDeclaration() == pModDeclaration);
			if (mod is IDecoratePanel decoratePanel)
			{
				decoratePanel.DecoratePanel(this);
			}
		}
	}

	private static void _init()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		GameObject val = new GameObject("ModInfoPanel", new Type[1] { typeof(RectTransform) });
		APrefab<ModInfoPanel>.Prefab = val.AddComponent<ModInfoPanel>();
	}
}
