namespace TripleBladeHorse.Combat
{
	[System.Serializable]
	public struct AttackResult
	{
		public bool _attackSuccess;
		public float _finalDamage;
		public float _finalFatigue;
		public bool _isWeakspot;
		public IAttackable _attackable;

		public static AttackResult Failed = new AttackResult();

		public AttackResult(IAttackable attackable, bool success, float damage, float fatigue, bool weakspot)
		{
			_attackable = attackable;
			_attackSuccess = success;
			_finalDamage = damage;
			_finalFatigue = fatigue;
			_isWeakspot = weakspot;
		}
	}
}