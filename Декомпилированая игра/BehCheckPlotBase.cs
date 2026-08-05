using ai.behaviours;

public class BehCheckPlotBase : BehaviourActionActor
{
	public override bool shouldRetry(Actor pActor)
	{
		if (base.shouldRetry(pActor))
		{
			return true;
		}
		if (pActor.hasPlot())
		{
			PlotRetryAction plot_retry_action = pActor.plot.getAsset().getPlotGroup().plot_retry_action;
			if (plot_retry_action != null && plot_retry_action())
			{
				return true;
			}
		}
		return false;
	}

	protected override void setupErrorChecks()
	{
		base.setupErrorChecks();
		uses_plots = true;
		uses_clans = true;
	}

	public override BehResult execute(Actor pActor)
	{
		if (!pActor.hasClan())
		{
			return BehResult.Stop;
		}
		if (!pActor.plot.isActive())
		{
			return BehResult.Stop;
		}
		return BehResult.Continue;
	}

	protected bool isBasePlotCheckOk(Actor pActor)
	{
		if (!pActor.hasPlot())
		{
			return false;
		}
		if (!pActor.isKingdomCiv())
		{
			return false;
		}
		Plot tPlot = pActor.plot;
		if (!tPlot.isActive())
		{
			return false;
		}
		PlotAsset tPlotAsset = tPlot.getAsset();
		if (!tPlotAsset.isAllowedByWorldLaws())
		{
			return false;
		}
		PlotCheckerDelegate tChecker = tPlotAsset.check_should_continue;
		if (tChecker != null && !tChecker(pActor))
		{
			return false;
		}
		return true;
	}
}
