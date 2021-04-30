using UnityEngine;
using Nav;
public class test2 : MonoBehaviour
{
	Navigation navigation;
	public Transform player;
	// Start is called before the first frame update
	void Start() {
		navigation = GetComponent<Navigation>();
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Q)) {
			//Debug.Log(navigation.player.transform.position);
			navigation.SetDestination("Larisa Thessaly", 2);
			//cities = new city();
		}
		//navigation.SetDestination("Larisa Thessaly", 2);
		//navigation.SetDestination("Tisaia",2);
		//navigation.SetDestination("Iolcus", 2);
		//navigation.Setdestination("Iolcus");
	}
	
}
