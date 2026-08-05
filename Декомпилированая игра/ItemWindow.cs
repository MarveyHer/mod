using UnityEngine;
using UnityEngine.UI;

public class ItemWindow : WindowMetaGeneric<Item, ItemData>
{
	[SerializeField]
	private EquipmentBanner _item_banner;

	[SerializeField]
	private Text _text_item_type;

	[SerializeField]
	private Text _text_item_description;

	[SerializeField]
	private SwitchButton _button_cursed;

	[SerializeField]
	private SwitchButton _button_eternal;

	[SerializeField]
	private Sprite _frame_sprite_legendary;

	[SerializeField]
	private Sprite _frame_sprite_epic;

	private IconOutline _outline;

	public override MetaType meta_type => MetaType.Item;

	protected override Item meta_object => SelectedMetas.selected_item;

	public void clickReforge()
	{
		meta_object.reforge(1);
		meta_object.addMod("divine_rune");
		updateStates();
	}

	public void clickReforgeDivine()
	{
		meta_object.reforge(30);
		meta_object.addMod("divine_rune");
		updateStates();
	}

	public void clickCursed()
	{
		if (meta_object.hasMod("cursed"))
		{
			meta_object.removeMod("cursed");
		}
		else
		{
			meta_object.addMod("cursed");
		}
		meta_object.addMod("divine_rune");
		updateStates();
	}

	public void clickEternal()
	{
		if (meta_object.hasMod("eternal"))
		{
			meta_object.removeMod("eternal");
		}
		else
		{
			meta_object.addMod("eternal");
		}
		meta_object.addMod("divine_rune");
		updateStates();
	}

	public void clickTransmutation()
	{
		meta_object.transmute();
		meta_object.addMod("divine_rune");
		updateStates();
	}

	private void updateStates()
	{
		showTopPartInformation();
		loadNameInput();
		updateStatsRows();
		AchievementLibrary.godly_smithing.check();
		Actor tActor = meta_object.getActor();
		if (!tActor.isRekt())
		{
			tActor.setStatsDirty();
		}
	}

	protected override void showTopPartInformation()
	{
		base.showTopPartInformation();
		clear();
		Item tItem = meta_object;
		_item_banner.load(tItem);
		EquipmentAsset tAsset = meta_object.getAsset();
		string tMat = "";
		if (tAsset.material != "basic")
		{
			tMat = tMat + "(" + LocalizedTextManager.getText(tAsset.getMaterialID()) + ") ";
		}
		_text_item_type.GetComponent<LocalizedText>().setKeyAndUpdate(tItem.getItemKeyType());
		_text_item_type.text = tMat + _text_item_type.text;
		_text_item_type.color = Toolbox.makeColor(meta_object.getQualityColor());
		_text_item_description.text = tItem.getItemDescription();
		_button_cursed.setEnabled(meta_object.hasMod("cursed"));
		_button_eternal.setEnabled(meta_object.hasMod("eternal"));
	}

	internal override void showStatsRows()
	{
		Item tItem = meta_object;
		BaseStatsHelper.showItemModsRows(base.getStatRow, tItem);
		BaseStatsHelper.showBaseStatsRows(base.getStatRow, tItem.getFullStats());
		showStatRow("durability", tItem.getDurabilityString(), MetaType.None, -1L);
		if (tItem.data.kills > 0)
		{
			showStatRow("creature_statistics_kills", tItem.data.kills, MetaType.None, -1L);
		}
	}

	private void showOutline()
	{
		_outline.show(RarityLibrary.legendary.color_container);
	}

	protected override void loadNameInput()
	{
		_name_input.inputField.onEndEdit.AddListener(delegate(string pString)
		{
			onNameChange(pString);
		});
		string tName = (_initial_name = meta_object.getName(pWithMaterial: false).Trim());
		_name_input.setText(tName);
		_name_input.textField.color = Toolbox.makeColor(meta_object.getQualityColor());
		if (meta_object.data.custom_name)
		{
			_name_input.SetOutline();
		}
	}
}
