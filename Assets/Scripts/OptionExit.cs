using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class OptionExit : Option{
	public override void SelectionPressed() {
		base.SelectionPressed();

		//GameManager Restart
		GameManager.Instance.ToMainMenu(true);
	}
}

