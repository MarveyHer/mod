using System.Collections.Generic;
using ai.behaviours;
using UnityPools;

public class BehTryNewPlot : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		if (pActor.hasPlot())
		{
			return BehResult.Continue;
		}
		if (pActor.isFighting())
		{
			return BehResult.Continue;
		}
		using ListPool<PlotAsset> tTempPotPlots = new ListPool<PlotAsset>();
		fillRandomPlots(pActor, tTempPotPlots);
		if (tTempPotPlots.Count == 0)
		{
			return BehResult.Continue;
		}
		startPlotFromTheList(pActor, tTempPotPlots);
		return BehResult.Continue;
	}

	private void fillRandomPlots(Actor pActor, ListPool<PlotAsset> pPotPlots)
	{
		fillPlotsToTry(pActor, AssetManager.plots_library.basic_plots, pPotPlots);
		if (pActor.hasReligion() && WorldLawLibrary.world_law_rites.isEnabled())
		{
			fillPlotsToTry(pActor, pActor.religion.possible_rites, pPotPlots);
		}
		pPotPlots.Shuffle();
	}

	private void fillPlotsToTry(Actor pActor, List<PlotAsset> pPlotList, ListPool<PlotAsset> pPotPossiblePlots)
	{
		for (int i = 0; i < pPlotList.Count; i++)
		{
			PlotAsset tAsset = pPlotList[i];
			if (tAsset.checkIsPossible(pActor))
			{
				pPotPossiblePlots.AddTimes(tAsset.pot_rate, tAsset);
			}
		}
	}

	private void startPlotFromTheList(Actor pActor, ListPool<PlotAsset> pPotList)
	{
		HashSet<PlotAsset> tChecked = UnsafeCollectionPool<HashSet<PlotAsset>, PlotAsset>.Get();
		for (int i = 0; i < pPotList.Count; i++)
		{
			PlotAsset tPlotAsset = pPotList[i];
			if (!tChecked.Contains(tPlotAsset))
			{
				if (BehaviourActionBase<Actor>.world.plots.tryStartPlot(pActor, tPlotAsset))
				{
					break;
				}
				tChecked.Add(tPlotAsset);
			}
		}
		UnsafeCollectionPool<HashSet<PlotAsset>, PlotAsset>.Release(tChecked);
	}
}
