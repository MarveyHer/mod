using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class FieldInfoList : MonoBehaviour
{
	public static string color_null = "#9F9F9F";

	public static string color_white = Toolbox.colorToHex(Toolbox.color_white);

	public static string color_string = "#F3961F";

	public static string color_enum = Toolbox.colorToHex(Toolbox.color_plague);

	public static string color_type = Toolbox.colorToHex(Toolbox.color_yellow);

	public static string color_collection = color_null;

	public static Dictionary<string, string> selected_field_data;

	public KeyValueField field_prefab;

	public InputField search_input_field;

	public Transform fields_transform;

	private ObjectPoolGenericMono<KeyValueField> _pool_fields;

	internal List<FieldInfo> field_infos = new List<FieldInfo>();

	internal Dictionary<string, FieldInfoListItem> fields_collection_data = new Dictionary<string, FieldInfoListItem>();

	public void init<T>() where T : class
	{
		init<T>(null);
	}

	public void init<T>(ListPool<string> pFieldsToLoad) where T : class
	{
		checkInitPool();
		field_infos.Clear();
		fields_collection_data.Clear();
		FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		Array.Reverse(fields);
		bool tUseFilter = pFieldsToLoad != null && pFieldsToLoad.Count > 0;
		int i = 0;
		FieldInfo[] array = fields;
		foreach (FieldInfo tField in array)
		{
			if (!tUseFilter || pFieldsToLoad.Contains(tField.Name))
			{
				field_infos.Add(tField);
				i++;
			}
		}
		if (search_input_field != null)
		{
			search_input_field.onValueChanged.AddListener(setDataSearched);
		}
	}

	public void checkInitPool()
	{
		if (_pool_fields == null)
		{
			_pool_fields = new ObjectPoolGenericMono<KeyValueField>(field_prefab, fields_transform);
		}
		else
		{
			clear();
		}
	}

	public void setData(object pReference)
	{
		foreach (FieldInfo tField in field_infos)
		{
			FieldInfoListItem tItem = getFieldData(tField, pReference);
			fields_collection_data.Add(tItem.field_name, tItem);
			addRow(tItem.field_name, tItem.field_value);
		}
	}

	public FieldInfoListItem getFieldData(FieldInfo pField, object pReference)
	{
		string tValueString = "";
		object tValue = pField.GetValue(pReference);
		Type tType = pField.FieldType;
		Dictionary<string, string> tCollectionContent = null;
		if (tValue != null)
		{
			if (!(tValue is bool tBool))
			{
				if (!(tValue is string tStr))
				{
					if (!(tValue is int tInt))
					{
						if (!(tValue is float tFloat))
						{
							if (!(tValue is Vector2 tVector))
							{
								if (!(tValue is Vector2Int tVectorInt))
								{
									if (!(tValue is Enum tEnum))
									{
										if (!(tValue is Array tArray))
										{
											if (!(tValue is IList tList))
											{
												if (tValue is IDictionary tDict)
												{
													tCollectionContent = dictionaryToRows(tDict);
													Type[] genericArguments = tType.GetGenericArguments();
													string tDKey = Toolbox.coloredText(genericArguments[0].Name, color_type);
													string tDValue = Toolbox.coloredText(genericArguments[1].Name, color_type);
													string tDCount = Toolbox.coloredText(tDict.Count.ToString(), color_white);
													tValueString = Toolbox.coloredText("Dictionary<" + tDKey + ", " + tDValue + ">[" + tDCount + "]", color_collection);
												}
												else if (tType.IsGenericType && typeof(HashSet<>) == tType.GetGenericTypeDefinition())
												{
													tCollectionContent = enumerableToRows(tValue as IEnumerable);
													string tSValue = Toolbox.coloredText(tType.GetGenericArguments()[0].Name, color_type);
													string tSCount = Toolbox.coloredText(tType.GetProperty("Count").GetValue(tValue).ToString(), color_white);
													tValueString = Toolbox.coloredText("HashSet<" + tSValue + ">[" + tSCount + "]", color_collection);
												}
												else
												{
													tValueString = Toolbox.coloredText(tValue.GetType().Name, color_type);
												}
											}
											else
											{
												tCollectionContent = enumerableToRowsCompacted(tList);
												string tLValue = Toolbox.coloredText(tType.GetGenericArguments()[0].Name, color_type);
												string tLCount = Toolbox.coloredText(tList.Count.ToString(), color_white);
												tValueString = Toolbox.coloredText("List<" + tLValue + ">[" + tLCount + "]", color_collection);
											}
										}
										else
										{
											tCollectionContent = enumerableToRowsCompacted(tArray);
											string tAValue = Toolbox.coloredText(tType.GetElementType().Name, color_type);
											string tACount = Toolbox.coloredText(tArray.Length.ToString(), color_white);
											tValueString = Toolbox.coloredText("Array<" + tAValue + ">[" + tACount + "]", color_collection);
										}
									}
									else
									{
										tValueString = Toolbox.coloredText($"{tType.Name}.{tEnum}", color_enum);
									}
								}
								else
								{
									string tVIntValueX = Toolbox.coloredText(tVectorInt.x.ToText(), color_white);
									string tVIntValueY = Toolbox.coloredText(tVectorInt.y.ToText(), color_white);
									tValueString = Toolbox.coloredText("Vector2Int(" + tVIntValueX + ", " + tVIntValueY + ")", color_collection);
								}
							}
							else
							{
								string tVValueX = Toolbox.coloredText(tVector.x.ToText() + "f", color_white);
								string tVValueY = Toolbox.coloredText(tVector.y.ToText() + "f", color_white);
								tValueString = Toolbox.coloredText("Vector2(" + tVValueX + ", " + tVValueY + ")", color_collection);
							}
						}
						else
						{
							tValueString = Toolbox.coloredText(tFloat.ToText() + "f", color_white);
						}
					}
					else
					{
						tValueString = Toolbox.coloredText($"{tInt}", color_white);
					}
				}
				else
				{
					string tQuoteSymbol = Toolbox.coloredText("\"", color_null);
					tValueString = Toolbox.coloredText(tQuoteSymbol + tStr + tQuoteSymbol, color_string);
				}
			}
			else
			{
				tValueString = Toolbox.coloredText($"{tBool}", tBool ? "#43FF43" : "#FB2C21");
			}
		}
		else
		{
			tValueString = Toolbox.coloredText("—", color_null);
		}
		return new FieldInfoListItem(pField.Name, tValueString, tCollectionContent);
	}

	public KeyValueField addRow(string pName, string pValue)
	{
		KeyValueField tNewRow = _pool_fields.getNext();
		tNewRow.name_text.text = pName;
		tNewRow.value.text = pValue;
		if (fields_collection_data.TryGetValue(pName, out var tItem))
		{
			Dictionary<string, string> tCollectionContent = tItem.collection_data;
			if (tCollectionContent == null || tCollectionContent.Count == 0)
			{
				tNewRow.value.GetComponent<TipButton>().enabled = false;
			}
			else
			{
				tNewRow.value.GetComponent<TipButton>().enabled = true;
				tNewRow.on_hover_value = delegate
				{
					selected_field_data = tCollectionContent;
				};
				tNewRow.on_hover_value_out = Tooltip.hideTooltip;
			}
		}
		return tNewRow;
	}

	internal void setDataSearched(string pValue)
	{
		clear();
		pValue = pValue.ToLower();
		if (string.IsNullOrEmpty(pValue))
		{
			int i = 0;
			{
				foreach (FieldInfoListItem tItem in fields_collection_data.Values)
				{
					KeyValueField tElement = addRow(tItem.field_name, tItem.field_value);
					setOddEvenColor(tElement, i);
					i++;
				}
				return;
			}
		}
		int k = 0;
		foreach (FieldInfoListItem tItem2 in fields_collection_data.Values)
		{
			if (tItem2.field_name.ToLower().Contains(pValue))
			{
				KeyValueField tElement2 = addRow(tItem2.field_name, tItem2.field_value);
				setOddEvenColor(tElement2, k);
				k++;
			}
		}
	}

	private void setOddEvenColor(KeyValueField pComponent, int pIndex)
	{
		if (pIndex % 2 == 0)
		{
			pComponent.setEvenColor();
		}
		else
		{
			pComponent.setOddColor();
		}
	}

	private Dictionary<string, string> enumerableToRowsCompacted(IEnumerable pEnumerable)
	{
		Dictionary<string, int> tCompacted = new Dictionary<string, int>();
		int i = 0;
		foreach (object item in pEnumerable)
		{
			string tKey = item.ToString();
			if (tCompacted.ContainsKey(tKey))
			{
				tCompacted[tKey]++;
				continue;
			}
			tCompacted.Add(tKey, 1);
			i++;
		}
		string tColorYellow = Toolbox.colorToHex(Toolbox.color_yellow);
		Dictionary<string, string> tResult = new Dictionary<string, string>();
		int k = 0;
		foreach (KeyValuePair<string, int> tPair in tCompacted)
		{
			string tValue = tPair.Value.ToString();
			tResult.Add(tPair.Key + "    ", Toolbox.coloredText("x      " + tValue, tColorYellow));
			k++;
		}
		return tResult;
	}

	private Dictionary<string, string> enumerableToRows(IEnumerable pEnumerable)
	{
		Dictionary<string, string> tResult = new Dictionary<string, string>();
		int i = 0;
		foreach (object tObject in pEnumerable)
		{
			tResult.Add($"[{i}]     ", tObject.ToString());
			i++;
		}
		return tResult;
	}

	private Dictionary<string, string> dictionaryToRows(IDictionary pDictionary)
	{
		Dictionary<string, string> tResult = new Dictionary<string, string>();
		foreach (object tKey in pDictionary.Keys)
		{
			tResult.Add($"[\"{tKey}\"]", pDictionary[tKey].ToString());
		}
		return tResult;
	}

	public void clear()
	{
		_pool_fields.clear();
	}
}
