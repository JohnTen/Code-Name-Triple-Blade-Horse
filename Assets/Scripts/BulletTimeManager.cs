using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;
using TripleBladeHorse;

public class BulletTimeManager : MonoBehaviour
{
	enum BTTriggeringType
	{
		Dash,
		Withdraw
	}

	[SerializeField] int _maxBTRetriggerTimes;
	[SerializeField] float _dashBTTriggerWindow;
	[SerializeField] float _withdrawBTTriggerWindow;
	[SerializeField] float _BTOverloadCooldown;
	[Header("Debug")]
	[SerializeField] BTTriggeringType _BTTrigger;
	[SerializeField] BTTriggeringType _lastBTTrigger;
	[SerializeField] bool _isInsideBulletTime;
	[SerializeField] int _BTTriggeredTimes;
	Timer _BTWindowTimer;
	Timer _BTOverloadTimer;

	public event Action OnTriggerBulletTimeFailed;

	private void Awake()
	{
		_BTWindowTimer = new Timer();
		_BTOverloadTimer = new Timer();
		TimeManager.Instance.OnBulletTimeBegin += HandleBulletTimeStart;
		TimeManager.Instance.OnBulletTimeEnd += HandleBulletTimeEnd;
	}

	private void OnDestroy()
	{
		_BTWindowTimer.Dispose();
		_BTOverloadTimer.Dispose();
	}

	public void StartDashBTWindow()
	{
		_BTTrigger = BTTriggeringType.Dash;
		_BTWindowTimer.Start(_dashBTTriggerWindow);
	}

	public void StartWithdrawBTWindow()
	{
		_BTTrigger = BTTriggeringType.Withdraw;
		_BTWindowTimer.Start(_withdrawBTTriggerWindow);
	}

	public void TriggerWithdrawBT()
	{
		if (!_BTOverloadTimer.IsReachedTime())
		{
			return;
		}

		if (_BTTriggeredTimes >= _maxBTRetriggerTimes &&
			_isInsideBulletTime)
		{
			print("Overload");
			_BTOverloadTimer.Start(_BTOverloadCooldown, ClearBTTriggeredTimes);
			OnTriggerBulletTimeFailed?.Invoke();
			return;
		}

		if (_isInsideBulletTime &&
			_lastBTTrigger == BTTriggeringType.Withdraw)
		{
			_BTTriggeredTimes++;
		}

		_lastBTTrigger = _BTTrigger;

		TimeManager.Instance.ActivateBulletTime();
	}

	public void TriggerDashBT()
	{
		_lastBTTrigger = _BTTrigger;

		TimeManager.Instance.ActivateBulletTime();
	}

	public void TriggerBulletTime()
	{
		if (_BTWindowTimer.IsReachedTime())
		{
			return;
		}

		if (_BTTrigger == BTTriggeringType.Dash)
		{
			TriggerDashBT();
		}
		else
		{
			TriggerWithdrawBT();
		}

		if (!_isInsideBulletTime && _BTTriggeredTimes > 0)
		{
			_BTTriggeredTimes--;
		}
	}

	private void HandleBulletTimeStart()
	{
		_isInsideBulletTime = true;
	}

	private void HandleBulletTimeEnd()
	{
		_isInsideBulletTime = false;
	}

	private void ClearBTTriggeredTimes(Timer timer)
	{
		print("ClearBTT");
		_BTTriggeredTimes = 0;
	}
}
