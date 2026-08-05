using UnityEngine;

public class CitiesKingdomsContainersController : MonoBehaviour
{
	[SerializeField]
	private CitiesBannersContainer _banners_cities;

	[SerializeField]
	private GameObject _line_cities;

	[SerializeField]
	private KingdomsBannersContainer _banners_kingdoms;

	[SerializeField]
	private GameObject _line_kingdoms;

	public void update(NanoObject pNano)
	{
		_banners_cities.update(pNano);
		_banners_kingdoms.update(pNano);
		IMetaObject obj = (IMetaObject)pNano;
		bool tHasCities = obj.hasCities();
		_banners_cities.gameObject.SetActive(tHasCities);
		_line_cities.SetActive(tHasCities);
		bool tHasKingdoms = obj.hasKingdoms();
		_banners_kingdoms.gameObject.SetActive(tHasKingdoms);
		_line_kingdoms.SetActive(tHasKingdoms);
	}
}
