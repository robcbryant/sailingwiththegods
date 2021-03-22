using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

namespace nav {
	public class Navigation : MonoBehaviour
	{
		public GameObject TitleScreen;
		Transform player;
		private city cities;
		[SerializeField]
		private NavMeshAgent AI;
		[SerializeField]
		//private LineRenderer lineRenderer;
		public TMP_Text text;
		public Image ImageSlot;
		private const string ResourcePath = "crew_portraits";
		private const string DefaultPortrait = "crew_portraits/phoenician_sailor";
		public GameObject menu;
		public GameObject Navgater;
		private bool StartNavigation = false;
		Vector3 nexPoint;
		private void Start() {
			cities = new city();
			menu.SetActive(false);
			Navgater.SetActive(false);
			if (AI == null) {
				AI = GetComponent<NavMeshAgent>();
			}
			//if(lineRenderer == null) {
			//	lineRenderer = GetComponent<LineRenderer>();
			//}
		}
		private void Update() {

			if(!TitleScreen.activeSelf && StartNavigation) {
				ShowMenu(StartNavigation);
			}
		}
		public static float CalcAngle(Vector3 from, Vector3 to) {
			Vector3 delta = from - to;
			return Mathf.Atan2(delta.x, delta.z) * 180 / Mathf.PI;
		}
		public static float EulerNormalize(float angle) {
			while (angle < -180) angle += 360;
			while (angle > 180) angle -= 360;
			return angle;
		}
		public void Setdestination(string target, int ID) {
			if (target == null) {
				Debug.LogError("Didn't find the city");
				return;
			}
			Vector3 targetlocaion = cities.GetCityLocation(target);
			if (targetlocaion == null) {
				Debug.LogError("Unable to find city location");
				return;
			}
			//assign image
			ImageSlot.sprite = Resources.Load<Sprite>(ResourcePath + "/" + ID) ?? Resources.Load<Sprite>(DefaultPortrait);
			StartNavigation = true;
			//find player
			player = GameObject.Find("playerShip").transform.Find("ShipParent").transform.Find("kyrenia_parent").transform;
			this.transform.position = player.position;
			float distance = Vector3.Distance(targetlocaion, player.position);
			float radius = 2;
			if (distance >= radius) {
				string Saildirecation = FindPlayerDirection(player); // find what angle is player faceing
				string Citydirecation = FindCityDirection(targetlocaion,player); // find city angle by the world
				string WhereToSail = getAIPath(targetlocaion, radius); // find the path
				text.text = Citydirecation +Saildirecation + WhereToSail; // assign text
			}
			if (distance < radius) {
				text.text = "You have arrive to " + target + ".";
			}
		}
		string FindCityDirection(Vector3 targetlocaion, Transform current) {
			float angle = CalcAngle(current.position + -current.up, targetlocaion);    // target angle relative to world
			angle -= 180;
			string citydirecation = "The city is in the "+ FindDirection(angle)+".";
			return citydirecation;
		}
		string FindPlayerDirection(Transform current) {
			float angle2 = CalcAngle(current.position, current.position + current.up); // player angle relative to world
			string Saildirecation = "The Ship is sail to the " + FindDirection(angle2) + ".";
			return Saildirecation;
		}
		string getAIPath(Vector3 target,float radius) {
			AI.stoppingDistance = radius;
			AI.SetDestination(target);
			Vector3[] path = AI.path.corners;
			//lineRenderer.positionCount = AI.path.corners.Length;
			if (path.Length <= 1) {
				return "";
			}
			//for (int i = 0; i < path.Length; i++) {
			//	lineRenderer.SetPosition(i, path[i]);
			//}
			//get next point
			if(path.Length > 3) {
				for (int i = 1; i < AI.path.corners.Length; i++) {
					float dis = Vector3.Distance(player.position, AI.path.corners[i]);
					if (dis < 6) {
						int z = (i + 1) <= AI.path.corners.Length ? i + 1 : (AI.path.corners.Length - 1);
						for (int j = i; j <= AI.path.corners.Length - 1; j++) {
							var tmp = (j + 1) <= (AI.path.corners.Length - 1) ? Vector3.Distance(AI.path.corners[j], AI.path.corners[j + 1]) : Vector3.Distance(AI.path.corners[j], AI.path.corners[AI.path.corners.Length - 1]);
							if (tmp > 5) {
								z = (j + 1) <= AI.path.corners.Length ? j + 1 : (AI.path.corners.Length - 1);
								break;
							}
						}
						nexPoint = AI.path.corners[z];
						break;
					}
					nexPoint = AI.path.corners[i];
					break;
				}
			}
			else {
				nexPoint = AI.path.corners[1];
			}
			//assign angle by the dircation
			Vector3 dir = (nexPoint - transform.position).normalized;
			//Debug.DrawRay(transform.position, dir, Color.red);
			float angle = GetAngleFromVectorToFloat(dir);
			return "The ship should sail to the " + FindDirection(angle) + ".";
		}
		public static float GetAngleFromVectorToFloat(Vector3 dir) {
			dir = dir.normalized;
			float n = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
			EulerNormalize(n);
			return n;
		}

		//Compasss
		String FindDirection(float angle) {
			EulerNormalize(angle);
			if (angle > -30f && angle <30f) {
				return "North";
			}
			if (angle < -30f && angle > -70f) {
				return "North West";
			}
			if (angle < -70f && angle > -110f) {
				return "West";
			}
			if (angle > 30f && angle < 70f) {
				return "North East";
			}
			if (angle > 70f && angle < 110f) {
				return"East";
			}
			if (angle < -110f && angle > -160f) {
				return"South West";
			}
			if (angle > 110f && angle < 160f) {
				return"South East";
			}
			if (angle > 160f || angle < -160) {
				return"South";
			}
			return "Error";
		}

		void ShowMenu(bool show) {
			if (text.text.Length > 1 || text != null ) {
				Navgater.SetActive(show);
			}
		}


		// Buttom code
		public void ActiveText() {
			menu.SetActive(true);
		}
		public void DeactiveText() {
			menu.SetActive(false);
		}
	}

}

