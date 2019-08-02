using UnityEngine;

namespace TripleBladeHorse.Combat
{
    public class Sword : BaseWeapon
    {
        [SerializeField] Collider2D _triggerBox;
        [SerializeField] CharacterState _state;

        public override void Activate(AttackPackage attack, AttackMove move)
        {
            base.Activate(attack, move);

            if (_triggerBox)
                _triggerBox.enabled = true;
        }

        public override void Deactivate()
        {
            base.Deactivate();

            if (_triggerBox)
                _triggerBox.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var attackable = other.GetComponentInParent<IAttackable>();
            var attackDirection = _state._facingRight ? Vector2.right : Vector2.left;
            TryAttack(attackable, attackDirection);
        }
    }
}
