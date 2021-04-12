using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

namespace Nav {
	public class Navigation : MonoBehaviour
	{
		[SerializeField] private GameObject _TitleScreen;
		Transform player;
		private city cities;
		[SerializeField] private NavMeshAgent AI;
		private LineRenderer lineRenderer;
		[SerializeField] private TMP_Text _Text;
		[SerializeField] private Image _imageSlot;
		private const string _ResourcePath = "crew_portraits";
		private const string _DefaultPortrait = "crew_portraits/phoenician_sailor";
		[SerializeField] private GameObject _menu;
		[SerializeField] private GameObject _Navgater;
		[SerializeField] private bool _lineRendererON;
		private bool _startNavigation = false;
		bool _CoroutineOn = false;
		Vector3 nexPoint;
		String postion;
		int? _crewID;
		private void Start() {
			cities = new city();
			_menu.SetActive(false);
			_Navgater.SetActive(false);
			if (AI == null) {
				AI = GetComponent<NavMeshAgent>();
			}
			if (lineRenderer == null) {
				lineRenderer = GetComponent<LineRenderer>();
			}
		}
		private void Update() {

			if(!_TitleScreen.activeSelf && _startNavigation) {
				ShowMenu(_startNavigation);
			}
			if(postion != null) {
				SetDestination(postion, _crewID.Value);
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
		public void SetDestination(string target, int ID) {
			if (target == null) {
				Debug.LogError("Didn't find the city");
				return;
			}
			//check the target id if the id is save, save the id to postion
			if (postion != null || _crewID != null) {
				target = postion;
				ID = _crewID.Value;
			}
			if (postion == null || _crewID == null) {
				postion = target;
				_crewID = ID;
			}
			Vector3 targetlocaion = cities.GetCityLocation(target);
			if (targetlocaion == null) {
				Debug.LogError("Unable to find city location");
				return;
			}
			player = GameObject.Find("playerShip").transform.Find("ShipParent").transform.Find("kyrenia_parent").transform;
			this.transform.position = player.position;
			//assign image
			_imageSlot.sprite = Resources.Load<Sprite>(_ResourcePath + "/" + ID) ?? Resources.Load<Sprite>(_DefaultPortrait);
			//find player
			float distance = Vector3.Distance(targetlocaion, player.position);
			float radius = 2;
			if (distance >= radius) {
				_startNavigation = true;
				string saiDirection = FindPlayerDirection(player); // find what angle is player faceing
				//string cityDirecation = FindCityDirection(targetlocaion, this.transform); // find city angle/direaction by player is faceing
				string whereToSail = getAIPath(targetlocaion, radius); // find the path
				_Text.text = whereToSail + " "+saiDirection; // assign text

			}
			if (distance < radius) {
				_Text.text = "You have arrive to " + target + ".";
				_startNavigation =false;
				postion = null;
				_crewID = null;
				if (!_CoroutineOn) {
					StartCoroutine(CompeteNavgation());
				}
			}
		}
		IEnumerator CompeteNavgation() {
			_CoroutineOn = true;
			yield return new WaitForSeconds(3);
			_Text.text = "";
			_Navgater.SetActive(false);
			_CoroutineOn = false;
		}
		string FindCityDirection(Vector3 targetlocaion, Transform current) {
			//float angle = CalcAngle(current.position + -current.up, targetlocaion);    // target angle relative to world
			float angle = CalcAngle(current.position + -current.up, targetlocaion);    // target angle relative to world
			 //angle -= 180;
			string citydirecation = "The city is in the "+ FindDirection(angle)+".";
			return citydirecation;
		}
		string FindPlayerDirection(Transform current) {
			float angle2 = CalcAngle(current.position, current.position + current.up); // player angle relative to world
			//float angle2 = CalcAngle(Vector3.forward, current.position ); // player angle relative to world
			string Saildirecation = "Current direction of the ship is sail to the " + FindDirection(angle2) + ".";
			return Saildirecation;
		}
		string getAIPath(Vector3 target,float radius) {
			AI.stoppingDistance = radius;
			AI.SetDestination(target);
			Vector3[] path = AI.path.corners;
			lineRenderer.positionCount = AI.path.corners.Length;
			//NavMeshAgent.SetDestination is asynchronous, so "the path may not become available until after a few frames later" so check if there is a path first 
			if (path.Length <= 1) {
				return "";
			}
			if(_lineRendererON){
				for (int i = 0; i < path.Length; i++) {
				lineRenderer.SetPosition(i, path[i]);
			}
			}
			//get next point
			if (path.Length > 3) {
				nexPoint = path[ProcessNearbyPathSegment()];
			}
			else {
				nexPoint =path[1];
			}
			//assign angle by the dircation
			Vector3 dir = (nexPoint - (player.position + player.up)).normalized;
			//Vector3 dir = (nexPoint - Vector3.forward).normalized;
			//Debug.DrawRay(transform.position, dir, Color.red);
			float angle = GetAngleFromVectorToFloat(dir);
			return "In order to reach the destination the ship should sail to the " + FindDirection(angle) + ".";
		}

		//Find the price by distance
		public int FindPrice(string cityName) {
			Vector3 targetLocaion = cities.GetCityLocation(cityName);
			int price = 50;
			float distance = 0;
			int mile = 10;
			//check if city exist
			if (targetLocaion == null) {
				Debug.LogError("Unable to find city location");
				return 50;
			}
			//set the AI to the location then store the corners into path
			AI.SetDestination(targetLocaion);
			Vector3[] path = AI.path.corners;
			if (path.Length <= 1) {
				return price;
			}
			//loop the ever croners find the total distance
			for (int i = 0; i < path.Length - 1; i++) {
				if (distance <= 0) {
					distance = Vector3.Distance(path[i], path[i + 1]);
				}
				if (distance > 0) {
					distance += Vector3.Distance(path[i], path[i + 1]);
				}
			}
			//for ever mile the price increase
			if(distance > 10) {
				price *= Mathf.RoundToInt(distance / mile);
				if (price >= 2000)
					price = 2000;
			}
			return price;
		}
		public static float GetAngleFromVectorToFloat(Vector3 dir) {
			dir = dir.normalized;
			float n = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
			EulerNormalize(n);
			return n;
		}
		private int ProcessNearbyPathSegment() {
			for (int index = 1; index < AI.path.corners.Length; index++) { 
				float Distance = Vector3.Distance(player.position, AI.path.corners[index]);
				if (Distance < 6) {
					int TemSegment = (index + 1) <= AI.path.corners.Length ? index + 1 : (AI.path.corners.Length - 1);
					return TemSegment;
				}
			}
			return 1;
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
			if (_Text.text.Length > 2 || _Text != null ) {
				_Navgater.SetActive(show);
			}
		}
		
		// Buttom code
		public void ActiveText() {
			_menu.SetActive(true);
		}
		public void DeactiveText() {
			_menu.SetActive(false);
		}
	}

}

