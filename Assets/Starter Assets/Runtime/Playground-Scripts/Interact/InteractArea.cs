using System;
using UnityEngine;

namespace StarterAssets
{
    public class InteractArea : MonoBehaviour
    {
        private InteractHandler interactHandler;

        [SerializeField] private RectTransform NPCWorldCanvas;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (NPCWorldCanvas != null)
            {
                NPCWorldCanvas.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("NPCWorldCanvas is not assigned in the inspector.");
            }

            interactHandler = InteractHandler.Instance;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                NPCWorldCanvas.gameObject.SetActive(true);
                interactHandler.SetInteractType(InteractHandler.InteractType.Dialogue);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                NPCWorldCanvas.gameObject.SetActive(false);
                interactHandler.SetInteractType(InteractHandler.InteractType.None);
            }
        }
    }
}