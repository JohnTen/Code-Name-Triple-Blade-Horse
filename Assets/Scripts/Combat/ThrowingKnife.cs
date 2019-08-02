using JTUtility;
using System.Collections.Generic;
using TripleBladeHorse.VFX;
using UnityEngine;

namespace TripleBladeHorse.Combat
{
    public enum KnifeState
    {
        InSheath,
        Flying,
        Returning,
        Hover,
        Stuck,
    }

    public class ThrowingKnife : BaseWeapon
    {
        [SerializeField] float speed;
        [SerializeField] float bladeLength;
        [SerializeField] float backDistance;
        [SerializeField] float sinkingLength;
        [SerializeField] float hoveringDistance;
        [SerializeField] float hoveringDuration;
        [SerializeField] float hoveringRotateSpeed;
        [SerializeField] float normalAttackRate = 1;
        [SerializeField] float floatAttackRate = 0.2f;
        [SerializeField] float cooldown = 1f;
        [SerializeField] LayerMask rayCastMask;
        [Header("VFX")]
        [SerializeField] Transform flyingVFX;
        [SerializeField] SpriteGhostEffect chargeFlyingEffect;
        [Header("Debug")]
        [SerializeField] float traveledDistance;
        [SerializeField] float hoverTimer;
        [SerializeField] KnifeState state;
        [SerializeField] bool piercing;
        [SerializeField] AttackMove normalAttack;
        [SerializeField] AttackMove chargedAttack;
        [SerializeField] AttackMove floatAttack;
        [SerializeField] AttackMove drawStuckAttack;


        bool activated;
        Sheath sheath;
        ICanStickKnife stuckOn;
        IAttackable stuckAttack;

        public KnifeState State
        {
            get { return state; }
            set
            {
                if (state == value) return;
                var lastState = state;
                state = value;
                OnStateChanged?.Invoke(this, lastState, state);
            }
        }

        public ICanStickKnife StuckOn => stuckOn;
        public IAttackable StuckAttack => stuckAttack;

        public float PullingForce { get; set; }

        public bool Stuck { get; private set; }

        public float Cooldown => cooldown;

        public event Action<ThrowingKnife, KnifeState, KnifeState> OnStateChanged;

        private void Awake()
        {
            flyingVFX.SetParent(null);
        }

        private void Update()
        {
            switch (State)
            {
                case KnifeState.Flying: Flying(); break;
                case KnifeState.Returning: Returning(); break;
                case KnifeState.Hover: Hovering(); break;
            }
        }

        public void SetSheath(Sheath sheath)
        {
            this.sheath = sheath;
        }

        public bool Launch(Vector3 direction, bool isPiercing = false)
        {
            if (State != KnifeState.InSheath) return false;

            piercing = isPiercing;
            State = KnifeState.Flying;

            transform.position = sheath.LaunchPosition.position;
            transform.right = direction;

            _attackMove = isPiercing ? chargedAttack : normalAttack;
            _defaultType = isPiercing ? AttackType.ChargedRange : AttackType.Range;
            _baseAttackRate = normalAttackRate;
            Activate(AttackPackage.CreateNewPackage(), _attackMove);

            if (flyingVFX)
            {
                flyingVFX.SetParent(this.transform);
                flyingVFX.position = transform.position;
                flyingVFX.rotation = transform.rotation;

                List<ParticleSystem> particles = new List<ParticleSystem>();
                flyingVFX.GetComponentsInChildren(particles);

                foreach (var particle in particles)
                {
                    particle.Play();
                }
            }

            if (chargeFlyingEffect && piercing)
            {
                chargeFlyingEffect.StartDraw();
            }

            return true;
        }

        public bool Hover()
        {
            if (State != KnifeState.Flying) return false;

            piercing = false;
            State = KnifeState.Hover;

            _attackMove = floatAttack;
            _defaultType = AttackType.Float;
            _baseAttackRate = floatAttackRate;
            Activate(AttackPackage.CreateNewPackage(), _attackMove);

            if (flyingVFX)
                flyingVFX.SetParent(null);

            if (chargeFlyingEffect)
            {
                chargeFlyingEffect.StopDraw();
            }

            return true;
        }

        public bool Withdraw()
        {
            if (State == KnifeState.InSheath || State == KnifeState.Returning)
                return false;

            transform.parent = null;
            State = KnifeState.Returning;

            _attackMove = piercing ? chargedAttack : normalAttack;
            _defaultType = piercing ? AttackType.ChargedRange : AttackType.Range;
            _baseAttackRate = normalAttackRate;
            Activate(AttackPackage.CreateNewPackage(), _attackMove);

            if (stuckOn != null)
            {
                Vector2 toStuckOn = (stuckOn as Component).transform.position - this.transform.position;
                toStuckOn = toStuckOn.normalized * PullingForce;
                if (stuckOn.TryPullOut(this.gameObject, ref toStuckOn))
                {
                    var direction = sheath.transform.position - this.transform.position;
                    direction = DirectionalHelper.NormalizeHorizonalDirection(direction);
                    _defaultType = AttackType.StuckNDraw;
                    TryAttack(stuckAttack, direction);
                }

                stuckOn = null;
            }

            Returning();


            if (flyingVFX)
            {
                flyingVFX.SetParent(this.transform);
                flyingVFX.position = transform.position;
                flyingVFX.rotation = transform.rotation;
            }

            if (chargeFlyingEffect && piercing)
            {
                chargeFlyingEffect.StartDraw();
            }

            return true;
        }

        public void RetractInstantly()
        {
            Deactivate();
            transform.position = sheath.transform.position;
            sheath.PutBackKnife(this);
            if (stuckOn != null)
            {
                Vector2 force = Vector2.zero;
                stuckOn.Remove(this.gameObject);
            }
            stuckOn = null;
            stuckAttack = null;

            Stuck = false;
            hoverTimer = 0;
            traveledDistance = 0;
            State = KnifeState.InSheath;

            if (flyingVFX)
            {
                List<ParticleSystem> particles = new List<ParticleSystem>();
                flyingVFX.GetComponentsInChildren(particles);
                flyingVFX.SetParent(null);
                foreach (var particle in particles)
                {
                    particle.Stop();
                }
            }

            if (chargeFlyingEffect)
            {
                chargeFlyingEffect.StopDraw();
            }
        }

        private void Flying()
        {
            Physics2D.queriesHitTriggers = false;
            Physics2D.queriesStartInColliders = true;
            var distance = speed * TimeManager.PlayerDeltaTime;
            var leftDistance = hoveringDistance - traveledDistance;

            if (distance > leftDistance)
                distance = leftDistance;

            var hits = Physics2D.RaycastAll(transform.position, transform.right, distance + bladeLength, rayCastMask);
            transform.position += transform.right * distance;
            traveledDistance += distance;

            foreach (var hit in hits)
            {
                HandleFlyingCollision(hit);
            }

            if (traveledDistance >= hoveringDistance)
                Hover();
        }

        private void Returning()
        {
            var dir = sheath.transform.position - transform.position;
            var distance = speed * TimeManager.PlayerDeltaTime + bladeLength;

            if (dir.sqrMagnitude <= backDistance * backDistance)
            {
                if (flyingVFX)
                {
                    List<ParticleSystem> particles = new List<ParticleSystem>();
                    flyingVFX.GetComponentsInChildren(particles);
                    flyingVFX.SetParent(null);
                    foreach (var particle in particles)
                    {
                        particle.Stop();
                    }
                }

                if (chargeFlyingEffect)
                {
                    chargeFlyingEffect.StopDraw();
                }

                Deactivate();
                sheath.PutBackKnife(this);
                State = KnifeState.InSheath;

                hoverTimer = 0;
                Stuck = false;
                traveledDistance = 0;

                return;
            }

            transform.right = dir;

            Debug.DrawRay(transform.position, transform.right * distance, Color.red);

            Physics2D.queriesHitTriggers = false;
            Physics2D.queriesStartInColliders = true;
            var hits = Physics2D.RaycastAll(transform.position, transform.right, distance, rayCastMask);
            foreach (var hit in hits)
            {
                TryAttack(hit.collider.transform);
            }

            transform.position += transform.right * speed * TimeManager.PlayerDeltaTime;
        }

        private void Hovering()
        {
            hoverTimer += TimeManager.PlayerDeltaTime;
            transform.Rotate(0, 0, hoveringRotateSpeed * TimeManager.PlayerDeltaTime);

            if (hoverTimer > hoveringDuration)
                Withdraw();

            Physics2D.queriesHitTriggers = false;
            Physics2D.queriesStartInColliders = true;
            Debug.DrawRay(transform.position, transform.right * bladeLength, Color.red);
            var hits = Physics2D.RaycastAll(transform.position, transform.right, bladeLength, rayCastMask);
            foreach (var hit in hits)
            {
                TryAttack(hit.collider.transform);
            }
        }

        private bool TryAttack(Transform target)
        {
            var attackable = target.GetComponentInParent<IAttackable>();
            if (IsAttackable(attackable))
            {
                var direction = target.position - transform.position;
                direction = DirectionalHelper.NormalizeHorizonalDirection(direction);
                return TryAttack(attackable, direction);
            }

            return false;
        }

        private void HandleFlyingCollision(RaycastHit2D hit)
        {
            var stickable = hit.collider.GetComponent<ICanStickKnife>();
            var attackable = hit.collider.GetComponentInParent<IAttackable>();

            if (!piercing && !Stuck && stickable != null && stickable.TryStick(this.gameObject))
            {
                Stuck = true;
                State = KnifeState.Stuck;
                transform.position = (Vector3)hit.point + transform.right * sinkingLength;
                transform.SetParent(hit.collider.transform);
                stuckOn = stickable;
            }

            if (IsAttackable(attackable))
            {
                var direction = hit.collider.transform.position - transform.position;
                direction = DirectionalHelper.NormalizeHorizonalDirection(direction);

                if (Stuck)
                {
                    _defaultType = AttackType.StuckNDraw;
                    _attackMove = drawStuckAttack;
                }

                if (TryAttack(attackable, direction) && Stuck)
                {
                    stuckAttack = attackable;
                }
            }

            if (attackable == null && !Stuck)
            {
                print(hit.collider.name);
                Hover();
            }
        }

        private AttackPackage CreateNewPackage(AttackType type, AttackMove move)
        {
            var package = AttackPackage.CreateNewPackage();
            package._hitPointDamage.Base = _baseHitPointDamage;
            package._enduranceDamage.Base = _baseEnduranceDamage;
            package._attackType = type;
            package._faction = Faction.Player;
            package = move.Process(package);

            return package;
        }
    }
}