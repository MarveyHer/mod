using UnityEngine.UI;

public class ClanListElement : WindowListElementBase<Clan, ClanData>
{
	public Text text_name;

	public CountUpOnClick members;

	public CountUpOnClick dead;

	public CountUpOnClick age;

	public CountUpOnClick renown;

	public UiUnitAvatarElement avatarLoader;

	internal override void show(Clan pClan)
	{
		base.show(pClan);
		Actor tChief = pClan.getChief();
		if (tChief.isRekt())
		{
			avatarLoader.gameObject.SetActive(value: false);
		}
		else
		{
			avatarLoader.gameObject.SetActive(value: true);
			avatarLoader.show(tChief);
		}
		text_name.text = pClan.name;
		text_name.color = pClan.getColor().getColorText();
		members.setValue(pClan.countUnits());
		renown.setValue(pClan.getRenown());
		int tAges = pClan.getAge();
		age.setValue(tAges);
		dead.setValue((int)pClan.getTotalDeaths());
	}

	protected override void tooltipAction()
	{
		Tooltip.show(this, "clan", new TooltipData
		{
			clan = meta_object
		});
	}

	protected override ActorAsset getActorAsset()
	{
		return meta_object.getActorAsset();
	}
}
