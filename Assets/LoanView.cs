using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel; 
using UnityEngine;
using UnityEngine.UI;

public class LoanViewModel : Model
{
	GameVars GameVars => Globals.GameVars;

	public Loan Loan => GameVars.playerShipVariables.ship.currentLoan;
	public Loan NewLoan {
		get {
			//Setup the initial term to repay the loan
			float numOfDaysToPayOffLoan = 10;
			//Determine the base loan amount off the city's population
			float baseLoanAmount = 500 * (GameVars.currentSettlement.population / 1000);
			//If base loan amount is less than 200 then make it 200 as the smallest amount available
			if (baseLoanAmount < 200f) baseLoanAmount = 200f;
			//Determine the actual loan amount off the player's clout
			int loanAmount = (int)(baseLoanAmount + (baseLoanAmount * GameVars.GetOverallCloutModifier(GameVars.currentSettlement.settlementID)));
			//Determmine the base interest rate of the loan off the city's population
			float baseInterestRate = 10 + (GameVars.currentSettlement.population / 1000);
			//Determine finalized interest rate after determining player's clout
			float finalInterestRate = (float)System.Math.Round(baseInterestRate - (baseInterestRate * GameVars.GetOverallCloutModifier(GameVars.currentSettlement.settlementID)), 3);

			//Create the Loan object for our button to process		
			return new Loan(loanAmount, finalInterestRate, numOfDaysToPayOffLoan, GameVars.currentSettlement.settlementID);
		}
	}

	public bool IsAtOriginPort => GameVars.CheckIfShipBackAtLoanOriginPort();
	public Settlement OriginPort => Loan != null ? GameVars.GetSettlementFromID(Loan.settlementOfOrigin) : null;

	public void GUI_PayBackLoan() {
		var amountDue = Loan.GetTotalAmountDueWithInterest();

		//Pay the loan back if the player has the currency to do it
		if (GameVars.playerShipVariables.ship.currency > amountDue) {
			GameVars.playerShipVariables.ship.currency -= amountDue;
			GameVars.playerShipVariables.ship.currentLoan = null;
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You paid back your loan and earned a little respect!";
			//give a boost to the players clout for paying back loan
			GameVars.AdjustPlayerClout(3);

			NotifyAny();
		}
		else {
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You currently can't afford to pay your loan back! Better make some more money!";
		}

		NotifyAny();
	}

	public void GUI_TakeOutLoan() {
		var loanAmount = NewLoan.amount;
		var loan = NewLoan;

		GameVars.playerShipVariables.ship.currentLoan = loan;
		GameVars.playerShipVariables.ship.currency += loanAmount;
		GameVars.showNotification = true;
		GameVars.notificationMessage = "You took out a loan of " + loanAmount + " drachma! Remember to pay it back in due time!";

		NotifyAny();
	}
}

public class LoanView : ViewBehaviour<LoanViewModel>
{
	// subscreens
	[SerializeField] CurrentLoanView CurrentLoanView = null;
	[SerializeField] NewLoanView NewLoanView = null;
	[SerializeField] LoanIsElsewhereView LoanIsElsewhereView = null;

	public override void Bind(LoanViewModel model) {
		base.Bind(model);

		CurrentLoanView.Bind(model);
		NewLoanView.Bind(model);
		LoanIsElsewhereView.Bind(model);
	}

	protected override void Refresh(object sender, string propertyChanged) {
		base.Refresh(sender, propertyChanged);

		if(Model.Loan == null) {
			CurrentLoanView.gameObject.SetActive(false);
			NewLoanView.gameObject.SetActive(true);
			LoanIsElsewhereView.gameObject.SetActive(false);
		}
		else if (Model.IsAtOriginPort) {
			CurrentLoanView.gameObject.SetActive(true);
			NewLoanView.gameObject.SetActive(false);
			LoanIsElsewhereView.gameObject.SetActive(false);
		}
		else {
			CurrentLoanView.gameObject.SetActive(false);
			NewLoanView.gameObject.SetActive(false);
			LoanIsElsewhereView.gameObject.SetActive(true);
		}
	}
}
