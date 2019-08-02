using JTUtility;
using System.Collections.Generic;
using UnityEngine;

namespace TripleBladeHorse.Combat
{
    public class Sheath : MonoBehaviour
    {
        [SerializeField] int numberOfKnife;
        [SerializeField] Transform launchPosition;
        [SerializeField] Transform[] sheaths;
        [SerializeField] List<ThrowingKnife> knifesInSheath;
        [SerializeField] List<ThrowingKnife> allKnifes;
        [SerializeField] SheathMover mover;

        Func<float> chargeTimer;

        public float ReloadSpeed { get; set; }

        public Transform LaunchPosition => launchPosition;
        public int knifeCount => knifesInSheath.Count;

        public event Action<ThrowingKnife> OnRecievedKnife;

        int _currentSheathIndex;
        bool reloading;

        private void Awake()
        {
            knifesInSheath = new List<ThrowingKnife>();

            for (int i = 0; i < allKnifes.Count; i++)
            {
                allKnifes[i].SetSheath(this);
                knifesInSheath.Add(allKnifes[i]);
                mover.ActiveSheath(sheaths[i], 0.1f);
            }
        }

        private void Update()
        {
            if (chargeTimer != null)
            {
                mover.FloatScale = 1 - chargeTimer();
                mover.RadiusScale = 1 - chargeTimer();
                mover.RotationScale = 1 - chargeTimer();
            }
        }

        public void UpdateFacingDirection(bool right)
        {
            if (right && this.transform.eulerAngles.y != 0)
            {
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (!right && this.transform.eulerAngles.y == 0)
            {
                this.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }

        public void StartCharge(Func<float> timer)
        {
            chargeTimer = timer;
        }

        public void StopCharge()
        {
            chargeTimer = null;
            mover.FloatScale = 1;
            mover.RadiusScale = 1;
            mover.RotationScale = 1;
        }

        public ThrowingKnife TakeKnife(bool force)
        {
            if ((!force && reloading) || knifesInSheath.Count <= 0)
                return null;

            for (int i = 0; i < knifesInSheath.Count; i++)
            {
                var knife = knifesInSheath[i];
                var sheath = knife.transform.parent;
                if (sheath == null)
                {
                    Debug.Log(knife.name);
                    Debug.Break();
                }
                if (!mover.IsReady(sheath))
                    continue;

                mover.DeactiveSheath(sheath);
                knife.transform.SetParent(null);
                knifesInSheath.RemoveAt(i);
                return knife;
            }

            return null;
        }

        public void PutBackKnife(ThrowingKnife knife)
        {

            for (int i = 0; i < sheaths.Length; i++)
            {
                var index = (i + _currentSheathIndex) % sheaths.Length;
                if (sheaths[index].childCount != 0) continue;

                sheaths[index].position = knife.transform.position;
                sheaths[index].rotation = knife.transform.rotation;
                knife.transform.parent = sheaths[index];
                mover.ActiveSheath(sheaths[index], knife.Cooldown);
                break;
            }

            _currentSheathIndex++;
            _currentSheathIndex %= sheaths.Length;
            knifesInSheath.Add(knife);
            if (OnRecievedKnife != null)
                OnRecievedKnife.Invoke(knife);
        }
    }
}