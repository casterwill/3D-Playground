using UnityEngine;
using System.Collections;

namespace StarterAssets
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [SerializeField] private Color hitFlashColor;
        [SerializeField] private float hitFlashDuration = 1f;

        private Material[] materials;
        private Color originalColor;


        private void Awake()
        {
            materials = GetComponentInChildren<Renderer>().materials;
            originalColor = materials[0].color;
        }

        public void TakeDamage(int damage, Vector3 hitPoint)
        {
            Debug.Log($"{gameObject.name} took {damage} damage!");
            StartCoroutine(FlashRed());
        }

        private IEnumerator FlashRed()
        {
            foreach (var material in materials)
            {
                material.color = hitFlashColor;
            }

            yield return new WaitForSeconds(hitFlashDuration);

            foreach (var material in materials)
            {
                material.color = originalColor;
            }
        }
    }
}