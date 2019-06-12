using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using JTUtility;

public interface ICanMove
{
	void Move(Vector3 direction);

	event Action OnBlocked;
}

public interface ICanJump
{
	void Jump();

	event Action OnJump;
}

public interface ICanDash
{
	void Dash(Vector3 direction);

	event Action OnDashingBegin;
	event Action OnDashingDelayBegin;
	event Action OnDashingFinished;
}

public interface ICanDetectGround
{
	bool IsOnGround { get; }

	event Action OnLanding;
	event Action OnTakingOff;
	event Action OnStayGround;
}