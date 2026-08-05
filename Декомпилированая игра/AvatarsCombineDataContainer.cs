using System.Collections.Generic;

public class AvatarsCombineDataContainer
{
	private Dictionary<string, AvatarsCombineDataElement> _dict = new Dictionary<string, AvatarsCombineDataElement>();

	private List<AvatarsCombineDataElement> _list = new List<AvatarsCombineDataElement>();

	public void add(string pId, int pAmount)
	{
		AvatarsCombineDataElement tElement = new AvatarsCombineDataElement(_dict.Count + 1, pAmount);
		_dict.Add(pId, tElement);
		_list.Add(tElement);
	}

	public int getListIndex(int pIndex, string pId)
	{
		AvatarsCombineDataElement tElement = _dict[pId];
		int num = tElement.order_index - 1;
		int divisor = 1;
		for (int i = num + 1; i < _list.Count; i++)
		{
			divisor *= _list[i].total_amount;
		}
		return pIndex / divisor % tElement.total_amount;
	}

	public void clear()
	{
		_dict.Clear();
		_list.Clear();
	}

	public int totalCombinations()
	{
		int tResult = 1;
		for (int i = 0; i < _list.Count; i++)
		{
			tResult *= _list[i].total_amount;
		}
		return tResult;
	}
}
