
[System.Serializable]
public struct AttackResult
{
	public bool _attackSuccess;
	public float _finalDamage;
	public float _finalFatigue;
	public bool _isWeakspot;

	public static AttackResult Failed = new AttackResult();

	public AttackResult(bool success, float damage, float fatigue, bool weakspot)
	{
		_attackSuccess = success;
		_finalDamage = damage;
		_finalFatigue = fatigue;
		_isWeakspot = weakspot;
	}
}