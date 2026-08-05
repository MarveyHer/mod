using DG.Tweening;
using UnityEngine.UI;

public class CultureListElement : WindowListElementBase<Culture, CultureData>
{
	public new Text name;

	public CountUpOnClick textFollowers;

	public CountUpOnClick textCities;

	public CountUpOnClick textRenown;

	public CountUpOnClick textAge;

	public CountUpOnClick textBooks;

	internal override void show(Culture pCulture)
	{
		base.show(pCulture);
		name.text = pCulture.data.name;
		name.color = pCulture.getColor().getColorText();
		textAge.setValue(pCulture.getAge());
		textFollowers.setValue(pCulture.countUnits());
		textRenown.setValue(pCulture.getRenown());
		textCities.setValue(pCulture.countCities());
		textBooks.setValue(pCulture.books.count());
	}

	protected override void OnDisable()
	{
		textFollowers.DOKill();
		textCities.DOKill();
		textRenown.DOKill();
		textAge.DOKill();
		textBooks.DOKill();
		base.OnDisable();
	}

	protected override void tooltipAction()
	{
		Tooltip.show(this, "culture", new TooltipData
		{
			culture = meta_object
		});
	}

	protected override ActorAsset getActorAsset()
	{
		return meta_object.getActorAsset();
	}
}
