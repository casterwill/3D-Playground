// SwordTrailHitDetection.cs
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace StarterAssets
{
    public class SwordTrailHitDetection : MonoBehaviour
    {
        [SerializeField] private Transform swordTip;
        [SerializeField] private Transform swordBase;
        [SerializeField] private LayerMask hittableLayers;
        [SerializeField] private int damage = 15;
        [SerializeField] private float hitCooldown = 0.1f;

        private Vector3 previousTipPos;
        private Vector3 previousBasePos;
        private HashSet<Collider> hitThisSwing = new HashSet<Collider>();
        private float lastHitTime;

        private bool isSwinging = false;

        private ThirdPersonController TPSController;
        private UnityEvent OnLightAttackStartEvent;
        private UnityEvent OnLightAttackEndEvent;

        private void Start()
        {
            TPSController = GetComponentInParent<ThirdPersonController>();

            OnLightAttackStartEvent = TPSController.OnLightAttackStart;
            OnLightAttackStartEvent.AddListener(OnSwingStart);

            OnLightAttackEndEvent = TPSController.OnLightAttackEnd;
            OnLightAttackEndEvent.AddListener(OnSwingEnd);
        }

        private async Awaitable ContinuousSweepHitCheck()
        {
            while (isSwinging)
            {
                if (!swordTip.gameObject.activeInHierarchy) return;

                CheckSweepHit();

                previousTipPos = swordTip.position;
                previousBasePos = swordBase.position;

                await Awaitable.NextFrameAsync();
            }
        }

        public void OnSwingStart()
        {
            if (!isSwinging)
            {
                isSwinging = true;
                hitThisSwing.Clear();
                previousTipPos = swordTip.position;
                previousBasePos = swordBase.position;

                _ = ContinuousSweepHitCheck();
            }
        }

        public void OnSwingEnd()
        {
            isSwinging = false;
        }

        private void CheckSweepHit()
        {
            if (Time.time - lastHitTime < hitCooldown) return;

            // BoxCast along the sword arc
            Vector3 direction = (swordTip.position - previousTipPos);
            float distance = direction.magnitude;

            if (distance < 0.01f) return;

            Vector3 center = (swordTip.position + previousTipPos) * 0.5f;
            Vector3 halfExtents = new Vector3(0.1f, 0.1f, distance * 0.5f);
            Quaternion rotation = Quaternion.LookRotation(direction.normalized);

            var hits = Physics.BoxCastAll(center, halfExtents, direction.normalized, rotation, 0f, hittableLayers);

            foreach (var hit in hits)
            {
                if (hitThisSwing.Contains(hit.collider)) continue;

                hitThisSwing.Add(hit.collider);
                lastHitTime = Time.time;

                if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(damage, hit.point);
                    SpawnHitEffect(hit.point, hit.normal);
                }
            }
        }

        private void SpawnHitEffect(Vector3 point, Vector3 normal)
        {
            // Instantiate particle effect, screen shake, sound, etc.
        }
    }
}