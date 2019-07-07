using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class NewLoanView : ViewBehaviour<LoanViewModel>
{
	public StringView Amount;
	public StringView Due;
	public StringView TotalOwed;
	public StringView InterestRate;
	public ButtonView TakeLoanButton;

	public override void Bind(LoanViewModel model) {
		base.Bind(model);

		TakeLoanButton.Bind(ValueModel.New(new ButtonViewModel {
			OnClick = Model.GUI_TakeOutLoan
		}));
	}

	protected override void Refresh(object sender, string propertyChanged) {
		base.Refresh(sender, propertyChanged);

		Amount.Bind(ValueModel.New(Model.NewLoan.amount.ToString()));
		Due.Bind(ValueModel.New(Model.NewLoan.numOfDaysUntilDue.ToString()));
		TotalOwed.Bind(ValueModel.New(Model.NewLoan.GetTotalAmountDueWithInterest().ToString()));
		InterestRate.Bind(ValueModel.New(Model.NewLoan.interestRate.ToString()));
	}
}
