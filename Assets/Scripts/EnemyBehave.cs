using System.Collections;
using System.Collections.Generic;
using JTUtility;
using UnityEngine;

public enum EnemyInput
{
    Attack,
    Jump,
}

public class EnemyBehave : MonoBehaviour, ICharacterInput<EnemyInput>
{
    [SerializeField] GameObject _character;
    [SerializeField] float _alertArea = 0;
    [SerializeField] float _patrolArea = 0;
    [SerializeField] float _stopTime = 0;
    [SerializeField] float _error = 0;
    Vector2 move;
    Vector2 aim;
    bool pause = false;
    float _stopposition;
    float _stoprandomTime;
    Vector2 _bornPosition;
    public bool DelayInput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool BlockInput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public event Action<InputEventArg<EnemyInput>> OnReceivedInput;

    public Vector2 GetAimingDirection()
    {
        return aim;
    }

    public Vector2 GetMovingDirection()
    {
        return move;
    }

    public void MoveToPlayer()
    {
        move = this.transform.position - _character.transform.position;
        aim = this.transform.position - _character.transform.position;
    }
    public void Patrol()
    {
        float distance = this.transform.position.x - _bornPosition.x;

        if (pause && _stoprandomTime < Time.time)
        {
            _stopposition = Random.Range(-_patrolArea, _patrolArea);
            pause = false;

        }else if (!pause && (distance - _stopposition) < _error)
        {
            _stoprandomTime = Random.Range(0, _stopTime) + Time.time;
            pause = true;
        }
        if (!pause)
        {
            if (distance - _stopposition > 0)
            {
                move = Vector2.right;
                aim = move;
            }
            else if (distance - _stopposition < 0)
            {
                move = Vector2.left;
                aim = move;
            }
        }

        if (pause)
        {
            move = Vector2.zero;
        }


    }
    public void MeleeAttack()
    {
        aim = this.transform.position - _character.transform.position;
        OnReceivedInput?.Invoke(new InputEventArg<EnemyInput>(EnemyInput.Attack));
    }
    public bool AlertAction()
    {
        float distance;
        distance = Mathf.Abs(this.transform.position.x - _character.transform.position.x);
        if ( distance < _alertArea)
        {
            return true;
        }
        else return false;
    }

    public void Awake()
    {
        _stopposition = Random.Range(-_patrolArea, _patrolArea);
        _stoprandomTime = Random.Range(0,_stopTime) + Time.time;
        _bornPosition = this.transform.position;
    }

}
