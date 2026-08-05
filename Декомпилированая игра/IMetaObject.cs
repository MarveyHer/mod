using System.Collections.Generic;
using UnityEngine;

public interface IMetaObject : ICoreObject
{
	int getMaxPossibleLifespan()
	{
		int tCount = 0;
		int tMax = 0;
		foreach (Actor unit in getUnits())
		{
			int tLifespan = (int)unit.stats["lifespan"];
			if (tLifespan > tMax)
			{
				tMax = tLifespan;
			}
			tCount++;
		}
		if (tCount == 0)
		{
			return 100;
		}
		return tMax;
	}

	float getRatioAdults()
	{
		int tCount = 0;
		float tTotal = 0f;
		foreach (Actor unit in getUnits())
		{
			if (unit.isAdult())
			{
				tTotal += 1f;
			}
			tCount++;
		}
		if (tCount <= 0)
		{
			return 0f;
		}
		return tTotal / (float)tCount;
	}

	float getRatioMales()
	{
		int tCount = 0;
		float tTotal = 0f;
		foreach (Actor unit in getUnits())
		{
			if (unit.isSexMale())
			{
				tTotal += 1f;
			}
			tCount++;
		}
		if (tCount <= 0)
		{
			return 0f;
		}
		return tTotal / (float)tCount;
	}

	float getRatioFemales()
	{
		int tCount = 0;
		float tTotal = 0f;
		foreach (Actor unit in getUnits())
		{
			if (unit.isSexFemale())
			{
				tTotal += 1f;
			}
			tCount++;
		}
		if (tCount <= 0)
		{
			return 0f;
		}
		return tTotal / (float)tCount;
	}

	float getRatioChildren()
	{
		int tCount = 0;
		float tTotal = 0f;
		foreach (Actor unit in getUnits())
		{
			if (!unit.isAdult())
			{
				tTotal += 1f;
			}
			tCount++;
		}
		if (tCount <= 0)
		{
			return 0f;
		}
		return tTotal / (float)tCount;
	}

	float getRatioHoused()
	{
		int tCount = 0;
		float tTotal = 0f;
		foreach (Actor unit in getUnits())
		{
			if (unit.hasHouse())
			{
				tTotal += 1f;
			}
			tCount++;
		}
		if (tCount <= 0)
		{
			return 0f;
		}
		return tTotal / (float)tCount;
	}

	float getRatioHomeless()
	{
		int tCount = 0;
		float tTotal = 0f;
		foreach (Actor unit in getUnits())
		{
			if (!unit.hasHouse())
			{
				tTotal += 1f;
			}
			tCount++;
		}
		if (tCount <= 0)
		{
			return 0f;
		}
		return tTotal / (float)tCount;
	}

	float getRatioHungry()
	{
		int tCount = 0;
		float tTotal = 0f;
		foreach (Actor unit in getUnits())
		{
			if (unit.isHungry())
			{
				tTotal += 1f;
			}
			tCount++;
		}
		if (tCount <= 0)
		{
			return 0f;
		}
		return tTotal / (float)tCount;
	}

	float getRatioStarving()
	{
		int tCount = 0;
		float tTotal = 0f;
		foreach (Actor unit in getUnits())
		{
			if (unit.isStarving())
			{
				tTotal += 1f;
			}
			tCount++;
		}
		if (tCount <= 0)
		{
			return 0f;
		}
		return tTotal / (float)tCount;
	}

	float getRatioSick()
	{
		int tCount = 0;
		float tTotal = 0f;
		foreach (Actor unit in getUnits())
		{
			if (unit.isSick())
			{
				tTotal += 1f;
			}
			tCount++;
		}
		if (tCount <= 0)
		{
			return 0f;
		}
		return tTotal / (float)tCount;
	}

	float getRatioHappy()
	{
		int tCount = 0;
		float tTotal = 0f;
		foreach (Actor unit in getUnits())
		{
			if (unit.isHappy())
			{
				tTotal += 1f;
			}
			tCount++;
		}
		if (tCount <= 0)
		{
			return 0f;
		}
		return tTotal / (float)tCount;
	}

	float getRatioUnhappy()
	{
		int tCount = 0;
		float tTotal = 0f;
		foreach (Actor unit in getUnits())
		{
			if (unit.isUnhappy())
			{
				tTotal += 1f;
			}
			tCount++;
		}
		if (tCount <= 0)
		{
			return 0f;
		}
		return tTotal / (float)tCount;
	}

	MetaTypeAsset getMetaTypeAsset();

	bool hasUnits();

	int countUnits();

	IEnumerable<Actor> getUnits();

	Actor getRandomUnit();

	Actor getRandomActorForReaper();

	int countFamilies();

	IEnumerable<Family> getFamilies();

	bool hasFamilies();

	ActorAsset getActorAsset();

	Sprite getSpriteIcon();

	bool isCursorOver();

	void setCursorOver();

	ColorAsset getColor();

	MetaObjectData getMetaData();

	int getRenown();

	int getPopulationPeople();

	long getTotalKills();

	long getTotalDeaths();

	bool isSelected();

	Actor getOldestVisibleUnit();

	Actor getOldestVisibleUnitForNameplatesCached();

	bool hasCities();

	IEnumerable<City> getCities();

	bool hasKingdoms();

	IEnumerable<Kingdom> getKingdoms();

	bool hasDied();
}
