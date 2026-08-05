public class AllianceWindow : WindowMetaGeneric<Alliance, AllianceData>
{
	public NameInput mottoInput;

	public StatBar bar_experience;

	public override MetaType meta_type => MetaType.Alliance;

	protected override Alliance meta_object => SelectedMetas.selected_alliance;

	protected override void initNameInput()
	{
		base.initNameInput();
		mottoInput.addListener(applyInputMotto);
	}

	private void applyInputMotto(string pInput)
	{
		if (pInput != null && meta_object != null)
		{
			meta_object.data.motto = pInput;
		}
	}

	protected override void showTopPartInformation()
	{
		base.showTopPartInformation();
		Alliance tAlliance = meta_object;
		if (tAlliance != null)
		{
			mottoInput.setText(tAlliance.getMotto());
			mottoInput.textField.color = tAlliance.getColor().getColorText();
		}
	}

	internal override void showStatsRows()
	{
		Alliance tAlliance = meta_object;
		tryShowPastNames();
		showStatRow("founded", tAlliance.getFoundedDate(), MetaType.None, -1L, "iconAge");
		tryToShowActor("alliance_founder", tAlliance.data.founder_actor_id, tAlliance.data.founder_actor_name, null, "actor_traits/iconStupid");
		tryToShowMetaKingdom("alliance_founder_kingdom", tAlliance.data.founder_kingdom_id, tAlliance.data.founder_kingdom_name);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		mottoInput.inputField.DeactivateInputField();
	}
}
