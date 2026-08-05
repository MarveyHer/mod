using UnityEngine;

public class MusicBoxContainerTiles
{
	public int amount;

	public float percent;

	public bool enabled;

	public Vector2 cur_pan;

	private Vector2 _last_pan;

	private float _chunks;

	public MusicAsset asset;

	public void clear()
	{
		amount = 0;
		_last_pan.Set(-1f, -1f);
		_chunks = 0f;
	}

	public void count(int pAmount, float pWhereFromX, float pWhereFromY)
	{
		amount += pAmount;
		_chunks += 1f;
		_last_pan.x += pWhereFromX;
		_last_pan.y += pWhereFromY;
	}

	public void calculatePan()
	{
		_last_pan.x /= _chunks + 1f;
		_last_pan.y /= _chunks + 1f;
		if (_chunks == 0f)
		{
			cur_pan.Set(-1f, -1f);
		}
		else if (cur_pan.x == -1f && cur_pan.y == -1f)
		{
			cur_pan.Set(_last_pan.x, _last_pan.y);
		}
		else
		{
			cur_pan = Vector2.MoveTowards(cur_pan, _last_pan, 5f);
		}
	}
}
