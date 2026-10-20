using UnityEngine;
using System.Collections;

namespace StarterAssets
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [SerializeField] private Color hitColor;
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
                material.color = hitColor;
            }

            yield return new WaitForSeconds(0.1f);

            foreach (var material in materials)
            {
                material.color = originalColor;
            }
        }
    }
}