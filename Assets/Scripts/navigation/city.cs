using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nav
{
	public class city
	{
		private Dictionary<string, Vector3> DictCity;
		private GameObject tmp;
		// Start is called before the first frame update
		public city() {
			var tmp = GameObject.Find("Settlement Master List");
			do {
				if (tmp == null) {
					Debug.Log("Can't find the object");
				}
				else {
					DictCity = new Dictionary<string, Vector3>();
					foreach (Transform child in tmp.transform) {
						DictCity[child.name] = child.transform.position;
					}
					Debug.Log(DictCity.Count);
					break;
				}
			} while (tmp = null);
			
		}

		public Vector3 GetCityLocation(string CityName) {
			return DictCity[CityName];
		}
	}
}

