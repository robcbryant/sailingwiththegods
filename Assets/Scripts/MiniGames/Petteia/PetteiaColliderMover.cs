using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetteiaColliderMover : MonoBehaviour
{
	public bool destory;
	public PetteiaGameController p;
	public GameObject go;

    void Start()
    {
		p = GameObject.Find("board").GetComponent<PetteiaGameController>();
		destory = false;
    }

	//void OnTriggerEnter(Collider other) {
	//	if (other.CompareTag("PetteiaB")) {
	//		p.en.pieces.Remove(other.gameObject);
	//		p.en.d.PlayerCaptures();
	//	}

	//	Destroy(other.gameObject);
	//}
}

