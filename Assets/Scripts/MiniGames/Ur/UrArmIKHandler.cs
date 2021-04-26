//David Herrod
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrArmIKHandler : MonoBehaviour
{
	protected Animator animator;

	public bool ikActive = false;
	public bool leftHand = true;
	public Transform rightHandObj = null;
	public Transform lookObj = null;
	public UrCounter counterOnTheMove;
	private bool useIk = false;
	private float ikValue = 0.0f;

	void Start() {
		animator = GetComponent<Animator>();
	}

	//a callback for calculating IK
	void OnAnimatorIK() {
		if (animator) {
			//Debug.Log(ikValue);
			//if the IK is active, set the position and rotation directly to the goal. 
			if (ikActive) {

				if (useIk) {
					if (ikValue < 1) { ikValue += 0.05f; }
				}
				else {
					if (ikValue > 0) {
						ikValue -= 0.05f;
					}
						
							
							}


					// Set the look target position, if one has been assigned
					if (lookObj != null) {
					animator.SetLookAtWeight(ikValue);
					animator.SetLookAtPosition(lookObj.position);
				}

				// Set the right hand target position and rotation, if one has been assigned
				if (rightHandObj != null /*&& leftHand*/) {
					animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikValue);
					animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikValue);
					animator.SetIKPosition(AvatarIKGoal.LeftHand, rightHandObj.position);
					animator.SetIKRotation(AvatarIKGoal.LeftHand, rightHandObj.rotation);
				}
				//else if (rightHandObj != null && !leftHand) {
				//	animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikValue);
				//	animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikValue);
				//	animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
				//	animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
				//}

			}

			//if the IK is not active, set the position and rotation of the hand and head back to the original position
			else /*if(leftHand)*/{
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikValue);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikValue);
				animator.SetLookAtWeight(ikValue);
			}
			//else if(!leftHand) {
			//	animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikValue);
			//	animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikValue);
			//	animator.SetLookAtWeight(ikValue);
			//}
		}
	}

	void PickUpPiece() {
		counterOnTheMove.TileMT();
		useIk = true;

	}
	void PutDownPiece() {
		counterOnTheMove.TileMT();
		useIk = false;
	}


}
