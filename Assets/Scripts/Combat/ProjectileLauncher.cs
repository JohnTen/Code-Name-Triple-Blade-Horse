using UnityEngine;

namespace TripleBladeHorse.Combat
{
    public class ProjectileLauncher : MonoBehaviour
    {
        [SerializeField] Transform _attackPoint;
        [SerializeField] Projectile _prefab;
        [SerializeField] float _delay = 0.3334f;
        [SerializeField] ParticleSystem _launchEffect;
        [SerializeField] AttackMove _move;
        [SerializeField] Vector2 _launchDirection = Vector2.right;
        public AudioSource launchSound;
        public Transform Target { get; set; }
        public Vector2 LaunchDirection
        {
            get => _launchDirection;
            set => _launchDirection = value;
        }

        public void Launch()
        {
            if (_launchEffect != null)
                _launchEffect.Play();

            if (launchSound != null)
            {
                launchSound.Play();
                launchSound.volume = Random.Range(1.50f, 1.75f);
                launchSound.pitch = Random.Range(0.75f, 1.25f);
            }

            Invoke("StartAttack", _delay);
        }

        public void StartAttack()
        {
            var projectile = Instantiate(_prefab.gameObject, _attackPoint.position, Quaternion.identity).GetComponent<Projectile>();
            projectile.Target = Target;
            projectile.InitializeDirection(LaunchDirection);
            projectile.Activate(AttackPackage.CreateNewPackage(), _move);
        }

        public void Interrupt()
        {
            CancelInvoke();
            _launchEffect.Stop();
        }
    }
}
