using UnityEngine;

public class PetteiaIKHelper : MonoBehaviour
{
	private PetteiaGameController p;
	private Transform inital, final;
	public GameObject piece;
	public float increment = 0.5f;
	public bool onTheMove;
	public GameObject ikTarget;
	public PetteiaArmIKHandler armIK;
    // Start is called before the first frame update
    void Start()
    {
		onTheMove = false;
		p = GetComponent<PetteiaGameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(onTheMove && piece.GetComponent<MeshRenderer>().enabled == false ) {
			if ((/*Vector3.Distance(ikTarget.transform.position, piece.transform.parent.position) > 0.001f) &&*/ Input.GetKey(KeyCode.Mouse0)))
				ikTarget.transform.position = Vector3.MoveTowards(ikTarget.transform.position, piece.transform.position, (increment * Time.deltaTime));
			else {
				armIK.gameObject.GetComponent<Animator>().SetTrigger("MovementIsDone");
				//transform.position = piece.transform.parent.position;

				onTheMove = false;
			}
		}
		if(Input.GetKeyUp(KeyCode.Mouse0)) {
			//armIK.gameObject.GetComponent<Animator>().SetTrigger("MovementIsDone"); onTheMove = false;
		}
    }
	public void SetInital(Transform t) 
	{
		ikTarget.transform.position = t.position;
		inital = t;
		onTheMove = true;
		armIK.gameObject.GetComponent<Animator>().SetTrigger("BeginMovement");
	}
	public void SetFinal(Transform t) {
		final = t;
	}
	public void SetPiece(GameObject p) {
		piece = p;
	}


}
