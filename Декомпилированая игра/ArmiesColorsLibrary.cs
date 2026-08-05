public class ArmiesColorsLibrary : ColorLibrary
{
	public ArmiesColorsLibrary()
	{
		file_path = "colors/colors_general";
	}

	public override void init()
	{
		base.init();
		loadFromFile<ArmiesColorsLibrary>();
	}

	public override bool isColorUsedInWorld(ColorAsset pAsset)
	{
		foreach (Army tObject in World.world.armies)
		{
			if (checkColor(pAsset, tObject.data.color_id))
			{
				return true;
			}
		}
		return base.isColorUsedInWorld(pAsset);
	}
}
