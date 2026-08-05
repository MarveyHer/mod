using System;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
	public static int premium_restore_opened;

	public static int premium_restore_action_pressed;

	public static int premium_more_help_pressed;

	public void clickGenerateMap(string pValue)
	{
		World.world.clickGenerateNewMap();
	}

	public void clickPremiumButton()
	{
		ScrollWindow.showWindow("steam");
	}

	public void clickPossess()
	{
		using ListPool<Actor> tList = new ListPool<Actor>(SelectedUnit.getAllSelected().Count);
		foreach (Actor tActor in SelectedUnit.getAllSelected())
		{
			if (tActor.asset.allow_possession)
			{
				tList.Add(tActor);
			}
		}
		if (tList.Count != 0)
		{
			ControllableUnit.setControllableCreatures(tList);
			ScrollWindow.hideAllEvent();
		}
	}

	public void openUnitTabTraitsEditor()
	{
		ActionLibrary.openUnitWindow(SelectedUnit.unit);
		ScrollWindow.getCurrentWindow().tabs.showTab("Traits");
	}

	public void openUnitTabEquipmentEditor()
	{
		ActionLibrary.openUnitWindow(SelectedUnit.unit);
		ScrollWindow.getCurrentWindow().tabs.showTab("Equipment");
	}

	public void openUnitTabMind()
	{
		ActionLibrary.openUnitWindow(SelectedUnit.unit);
		ScrollWindow.getCurrentWindow().tabs.showTab("Mind");
	}

	public void openUnitTabGenealogy()
	{
		ActionLibrary.openUnitWindow(SelectedUnit.unit);
		ScrollWindow.getCurrentWindow().tabs.showTab("Genealogy");
	}

	public void openUnitTabPlot()
	{
		ActionLibrary.openUnitWindow(SelectedUnit.unit);
		ScrollWindow.getCurrentWindow().tabs.showTab("Plots");
	}

	public void openUnitSpectate()
	{
		World.world.followUnit(SelectedUnit.unit);
		ScrollWindow.hideAllEvent();
	}

	public void openSettings()
	{
		ScrollWindow.showWindow("settings");
	}

	public void openSavesList()
	{
		ScrollWindow.showWindow("saves_list");
	}

	public void openPremiumHelp()
	{
		premium_restore_opened++;
		ScrollWindow.showWindow("premium_help");
	}

	public void openPremiumHelpFaq()
	{
		premium_more_help_pressed++;
		if (Config.isAndroid || Config.isEditor)
		{
			Application.OpenURL("https://www.superworldbox.com/faq#i-purchased-the-premium-on-android-but-haven-t-received-it-or-you-trying-to-play-on-new-another-android-device-with-the-same-account");
		}
		else if (Config.isIos)
		{
			Application.OpenURL("https://www.superworldbox.com/faq#i-purchased-the-premium-on-ios-and-later-got-a-new-apple-device-how-do-i-restore-premium");
		}
	}

	public void openPatchNotes()
	{
		ScrollWindow.showWindow("patch_log");
		Analytics.LogEvent("open_link_changelog");
	}

	public void clickRewardAds()
	{
		if (!ScrollWindow.isCurrentWindow("reward_ads"))
		{
			ScrollWindow.showWindow("reward_ads");
		}
	}

	public void showWindow(string pID)
	{
		ScrollWindow.showWindow(pID);
	}

	public void locateSelectedVillage()
	{
		World.world.locateSelectedVillage();
	}

	public void locateSelectedUnit()
	{
		World.world.followUnit(SelectedUnit.unit);
		ScrollWindow.hideAllEvent();
	}

	public void locateSelectedArmy()
	{
		Army tArmy = SelectedMetas.selected_army;
		Actor tToFollow = ((!tArmy.hasCaptain()) ? tArmy.units.GetRandom() : tArmy.getCaptain());
		World.world.followUnit(tToFollow);
		ScrollWindow.hideAllEvent();
	}

	public void startLoadSaveSlot()
	{
		AutoSaveManager.autoSave(pSkipDelete: true);
		World.world.save_manager.startLoadSlot();
	}

	public void clickSaveSlot()
	{
		AutoSaveManager.resetAutoSaveTimer();
		World.world.save_manager.clickSaveSlot();
	}

	public void confirmDeleteWorld()
	{
		SaveManager.deleteCurrentSave();
	}

	public void startTutorialBear()
	{
		World.world.tutorial.startTutorial();
	}

	public void showRewardedAd()
	{
		PlayerConfig.instance.data.powerReward = string.Empty;
		if (Config.isMobile || Config.isEditor)
		{
			RewardedAds.instance.ShowRewardedAd("gift");
		}
	}

	public void showRewardedSaveSlotAd()
	{
		PlayerConfig.instance.data.powerReward = "saveslots";
		if (Config.isMobile || Config.isEditor)
		{
			RewardedAds.instance.ShowRewardedAd("save_slot");
		}
	}

	public void hideRewardWindowAndHighlightPower()
	{
		if (!ScrollWindow.isCurrentWindow("reward_ads_received"))
		{
			return;
		}
		ScrollWindow.get("reward_ads_received").clickHide();
		if (!(PlayerConfig.instance.data.lastReward != string.Empty))
		{
			return;
		}
		if (PlayerConfig.instance.data.lastReward.StartsWith("saveslots", StringComparison.Ordinal))
		{
			ScrollWindow.showWindow("saves_list");
		}
		else
		{
			PowerButton tButton = PowerButton.get(PlayerConfig.instance.data.lastReward);
			if (tButton == null)
			{
				return;
			}
			tButton.selectPowerTab();
		}
		PlayerConfig.instance.data.lastReward = string.Empty;
	}

	public void clickUnHideUI()
	{
		Config.ui_main_hidden = false;
	}

	public void closeActivePowerBar()
	{
		PowersTab.unselect();
	}

	public void clickOpenMain()
	{
		PowerTabAsset tAsset = PowersTab.getActiveTab().getAsset();
		tAsset.on_main_info_click(tAsset);
	}

	public void clickBackTab()
	{
		if (!SelectedTabsHistory.showPreviousTab())
		{
			PowerTabController.showMainTab();
		}
	}

	public void restorePurchases()
	{
		premium_restore_action_pressed++;
		InAppManager.instance.RestorePurchases();
	}

	public void debugUnlockAll()
	{
		GameProgress.instance.debugUnlockAll();
		PowerButton.checkActorSpawnButtons();
	}

	public void debugClearAllProgress()
	{
		GameProgress.instance.debugClearAll();
		PowerButton.checkActorSpawnButtons();
	}

	public void debugClearAchievements()
	{
		GameProgress.instance.debugClearAllAchievements();
		PowerButton.checkActorSpawnButtons();
	}

	public void debugUnlockAllAchievements()
	{
		GameProgress.instance.unlockAllAchievements();
		PowerButton.checkActorSpawnButtons();
	}

	public void debugClearBannedSignals()
	{
		foreach (SignalAsset item in AssetManager.signals.list)
		{
			item.unban();
		}
	}
}
