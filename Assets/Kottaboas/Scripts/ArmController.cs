using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
	public Animator AnimArm;

    // Start is called before the first frame update
    void Start()
    {
		AnimArm.SetBool("Grab_cup", true);

		AnimArm.SetFloat("Upper_arm_angle_UP_Down", -1f);
		//0 to 1
		AnimArm.SetFloat("Upper_arm_angle_forward", 1f);
    }

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyUp(KeyCode.Space)) {
			AnimArm.SetTrigger("Fling");
		}
	}

	public void ArmReset() 	{
		AnimArm.SetTrigger("Reset");
	}
}
