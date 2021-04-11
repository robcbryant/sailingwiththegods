using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nav
{
	public class city
	{
		private Dictionary<string, Vector3> DictCity;
		private GameObject tmp;
		// Start is called before the first frame update
		public city() {
			do {
				tmp = GameObject.Find("Settlement Master List");
				if (tmp == null) {
					Debug.Log("Can't find the object");
				}
				else {
					DictCity = new Dictionary<string, Vector3>();
					foreach (Transform child in tmp.transform) {
						DictCity[child.name] = child.transform.position;
					}
					Debug.Log(DictCity.Count);
					
				}
			} while (tmp = null);
			
		}

		public Vector3 GetCityLocation(string CityName) {
			return DictCity[CityName];
		}
	}
}

