using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverPoint : MonoBehaviour
{
	[SerializeField] Vector3 _position;
	[SerializeField] bool set;

	PlayerState _player;
	static RecoverPoint lastMajorRecoverPoint;
	static RecoverPoint lastMinorRecoverPoint;

	public static void MainRespawn(bool recover = false)
	{
		if (recover)
		{
			lastMajorRecoverPoint._player.transform.position = lastMajorRecoverPoint._position;
            lastMajorRecoverPoint._player._hitPoints.ResetCurrentValue();
        }
		else
		{
			lastMinorRecoverPoint._player.transform.position = lastMinorRecoverPoint._position;
		}
	}

	public void Respwn(bool major)
	{
		MainRespawn(major);
	}

    public void SetAsMajorPoint()
	{
		if (set) return;

		lastMajorRecoverPoint = this;
        lastMinorRecoverPoint = this;
        _position = transform.position;
	}

	public void SetAsMinorPoint()
	{
		if (set) return;

		lastMinorRecoverPoint = this;
		_position = transform.position;
	}

	private void Start()
	{
		_player = FindObjectOfType<PlayerState>();
	}
}
