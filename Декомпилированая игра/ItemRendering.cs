using UnityEngine;

public static class ItemRendering
{
	public static Sprite getItemMainSpriteFrame(IHandRenderer pHandRendererAsset)
	{
		if (pHandRendererAsset == null)
		{
			return null;
		}
		Sprite[] tSpriteList = pHandRendererAsset.getSprites();
		if (tSpriteList.Length > 1)
		{
			return AnimationHelper.getSpriteFromList(0, tSpriteList, 5f);
		}
		return tSpriteList[0];
	}
}
