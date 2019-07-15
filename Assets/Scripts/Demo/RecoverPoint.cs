using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse;

public class RecoverPoint : MonoBehaviour
{
	[SerializeField] Vector3 _position;
	[SerializeField] float _stamina;
	[SerializeField] bool _set;
	[SerializeField] GameObject[] respawnHandlers;

	PlayerState _player;
	static RecoverPoint lastMajorRecoverPoint;
	static RecoverPoint lastMinorRecoverPoint;

	private void OnDrawGizmosSelected()
	{
		foreach (var obj in respawnHandlers)
		{
			var handler = obj.GetComponent<ICanHandleRespawn>();

			if (handler != null)
				Gizmos.color = Color.green;
			else
				Gizmos.color = Color.red;

			Gizmos.DrawLine(this.transform.position, obj.transform.position);
		}
	}

	public static void MainRespawn(bool recover = false)
	{
		if (recover)
		{
			lastMajorRecoverPoint._player.transform.position = lastMajorRecoverPoint._position;
            lastMajorRecoverPoint._player._hitPoints.ResetCurrentValue();
			lastMajorRecoverPoint._player._endurance.ResetCurrentValue();
			lastMajorRecoverPoint._player._stamina.Current = lastMajorRecoverPoint._stamina;
			GameManager.PlayerInstance.ResetState();
			lastMajorRecoverPoint.InvokeSpawnHandlers();

		}
		else
		{
			lastMinorRecoverPoint._player.transform.position = lastMinorRecoverPoint._position;
			lastMajorRecoverPoint._player._stamina.Current = lastMajorRecoverPoint._stamina;
			GameManager.PlayerInstance.ResetState();
			lastMajorRecoverPoint.InvokeSpawnHandlers();
		}
	}

	public void Respwn(bool major)
	{
		MainRespawn(major);
	}

    public void SetAsMajorPoint()
	{
		if (_set) return;

		lastMajorRecoverPoint = this;
        lastMinorRecoverPoint = this;
        _position = transform.position;
		_stamina = _player._stamina;
	}

	public void SetAsMinorPoint()
	{
		if (_set) return;

		lastMinorRecoverPoint = this;
		_position = transform.position;
		_stamina = _player._stamina;
	}

	private void InvokeSpawnHandlers()
	{
		foreach (var obj in respawnHandlers)
		{
			var handler = obj.GetComponent<ICanHandleRespawn>();
			if (handler != null) handler.Respawn();
		}
	}

	private void Start()
	{
		_player = TripleBladeHorse.GameManager.PlayerInstance.GetComponent<PlayerState>();
	}
}
