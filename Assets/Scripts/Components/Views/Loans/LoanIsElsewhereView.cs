using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class LoanIsElsewhereView : ViewBehaviour<LoanViewModel>
{
	public StringView Amount;
	public StringView Due;
	public StringView Origin;

	protected override void Refresh(object sender, string propertyChanged) {
		base.Refresh(sender, propertyChanged);

		// this is null when you first pay back a loan and Model.Loan gets nulled out
		if (Model.Loan != null) {
			Amount.Bind(ValueModel.New(Model.Loan.amount.ToString()));
			Due.Bind(ValueModel.New(Model.Loan.numOfDaysUntilDue.ToString()));
			Origin.Bind(ValueModel.New(Model.OriginPort.name));
		}
	}
}
