using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TavernaMiniGameDialog : MonoBehaviour
{
	public GameObject textBackground;
	public Text enemyName;
	public Text dialog;
	public Image enemyImage;
	private const string ResourcePath = "crew_portraits";
	private const string DefaultPortrait = "crew_portraits/phoenician_sailor";

	private CrewMember crew;

    void Start()
    {
		textBackground.SetActive(false);

		if (Globals.GameVars != null) {
			crew = Globals.GameVars.currentSettlement.availableCrew.RandomElement();
			enemyName.text = crew.name;
			enemyImage.sprite = Resources.Load<Sprite>(ResourcePath + "/" + crew.ID) ?? Resources.Load<Sprite>(DefaultPortrait);
		}


		//enemyName.text = Globals.GameVars.currentSettlement.availableCrew.RandomElement<CrewMember>().name;
		//enemyName.text = Globals.GameVars.GetSettlementFromID(0).availableCrew.RandomElement<CrewMember>().name;
		//enemyFaces = Globals.GameVars.newGameAvailableCrew.RandomElement<CrewMember>();
		//Debug.Log(Globals.GameVars.newGameAvailableCrew.RandomElement<CrewMember>().name);

		//CrewMember c = Globals.GameVars.currentSettlement.availableCrew.RandomElement<CrewMember>();
		//enemyName.text = c.name;
		//enemyFaces.sprite = Resources.Load<Sprite>(ResourcePath + "/" + c.ID) ?? Resources.Load<Sprite>(DefaultPortrait);

	}

	public void DisplayInsult() {
		Time.timeScale = 0;
		textBackground.SetActive(true);
		if (Globals.GameVars != null) {
			dialog.text = Globals.GameVars.tavernaGameInsults.RandomElement();
		}
		else {
			dialog.text = "Insult goes here";
		}
	}

	public void DisplayBragging() {
		Time.timeScale = 0;
		textBackground.SetActive(true);
		if (Globals.GameVars != null) {
			dialog.text = Globals.GameVars.tavernaGameBragging.RandomElement();
		}
		else {
			dialog.text = "Bragging goes here";
		}
	}

	public void CloseDialog() {
		Time.timeScale = 1;
		textBackground.SetActive(false);
	}

	//public void EnemyCaptures() {
	//	if (Random.Range(0, 101) > 50) {
	//		StartCoroutine(EnemyCapturesFunction());
	//	}
	//}
	//public void PlayerCaptures() {
	//	if (Random.Range(0, 101) > 50) {
	//		StartCoroutine(PlayerCapturesFunction());
	//	}
	//}

	//IEnumerator EnemyCapturesFunction() {
	//	yield return new WaitForSeconds(1f);
	//	textBackground.SetActive(true);
	//	dialog.text = CaptureLines.RandomElement();
	//	//yield return new WaitForSeconds(8f);
		
	//	//EnemyCanvas.SetActive(false);
	//}

	//IEnumerator PlayerCapturesFunction() {
	//	yield return new WaitForSeconds(1f);
	//	textBackground.SetActive(true);
	//	dialog.text = getCapturedLines.RandomElement();
	//	//yield return new WaitForSeconds(8f);
		
	//	//EnemyCanvas.SetActive(false);
	//}
	//public void ResetBoard() {
	//	Scene scene = SceneManager.GetActiveScene();
	//	SceneManager.LoadScene(scene.name);
	//}
}
