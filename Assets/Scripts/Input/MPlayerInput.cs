using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse
{
	public class MPlayerInput : PlayerInput
	{
		protected override void HandleWithdraw()
		{
			var onButton = false; 

			if (IsUsingController)
			{
				onButton = _input.GetAxis("WithdrawOnAir") > 0.4f;

				if (onButton)
				{
					// Button Down
					if (!_withdrawPressedBefore)
					{
						InvokeInputEvent(PlayerInputCommand.WithdrawOne);
						_withdrawPressedBefore = true;
					}

					// Button hold
				}
				// Button Up
				else if (_withdrawPressedBefore)
				{
					_withdrawPressedBefore = false;
				}
			}
			else
			{
				onButton = _input.GetButtonDown("WithdrawOnAir") || _input.GetButtonDown("WithdrawStuck");
				if (onButton)
				{
					InvokeInputEvent(PlayerInputCommand.WithdrawOne);
				}
			}
		}
	}
}
