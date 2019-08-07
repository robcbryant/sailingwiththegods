using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TimePassingView : ViewBehaviour<IValueModel<float>>
{
	[SerializeField] StringView Message = null;

	public override void Bind(IValueModel<float> model) {
		base.Bind(model);

		Message?.Bind(model.Select(days => "Day " + string.Format("{0:0.00}", days)));
	}
}