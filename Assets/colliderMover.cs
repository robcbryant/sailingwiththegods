using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colliderMover : MonoBehaviour
{
	public bool destory;
	public PetteiaGameController p;
	public GameObject go;
    // Start is called before the first frame update
    void Start()
    {
		p = GameObject.Find("board").GetComponent<PetteiaGameController>();
		destory = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	void OnTriggerEnter(Collider other) {


		if (other.CompareTag("PetteiaB")) {
			p.en.pieces.Remove(other.gameObject);
			p.en.d.PlayerCaptures();
		}
		
		Destroy(other.gameObject);
	}
}

