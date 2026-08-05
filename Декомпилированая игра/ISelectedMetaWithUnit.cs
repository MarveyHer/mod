using UnityEngine;

public interface ISelectedMetaWithUnit
{
	SelectedMetaUnitElement unit_element { get; }

	GameObject unit_element_separator { get; }

	string unit_title_locale_key { get; }

	int last_dirty_stats_unit { get; set; }

	Actor last_unit { get; set; }

	bool checkUnitElement()
	{
		if (!hasUnit())
		{
			setUnitElementVisible(pState: false);
			return true;
		}
		setUnitElementVisible(pState: true);
		Actor tActor = getUnit();
		UiUnitAvatarElement tAvatar = unit_element.getAvatar();
		if (unitChanged(tActor) || tAvatar.avatarLoader.actorStateChanged())
		{
			unit_element.show(tActor, unit_title_locale_key);
			last_dirty_stats_unit = tActor.getStatsDirtyVersion();
			last_unit = tActor;
			return true;
		}
		tAvatar.updateTileSprite();
		return false;
	}

	void setUnitElementVisible(bool pState)
	{
		unit_element.gameObject.SetActive(pState);
		unit_element_separator.SetActive(pState);
	}

	void avatarTouch()
	{
		if (hasUnit())
		{
			Actor unit = getUnit();
			SelectedUnit.select(unit);
			SelectedObjects.setNanoObject(unit);
			PowerTabController.showTabSelectedUnit();
			((IShakable)ToolbarButtons.instance).shake();
		}
	}

	bool hasUnit();

	Actor getUnit();

	bool unitChanged(Actor pActor)
	{
		if (pActor.getStatsDirtyVersion() == last_dirty_stats_unit)
		{
			return pActor != last_unit;
		}
		return true;
	}

	void clearLastUnit()
	{
		last_unit = null;
		last_dirty_stats_unit = -1;
	}
}
