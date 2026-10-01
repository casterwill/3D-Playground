using System;
using UnityEngine;

namespace StarterAssets
{
    public class InteractHandler : MonoBehaviour
    {
        public enum InteractType
        {
            None,
            Dialogue,
            Quest,
            Shop
        }

        private InteractType currentInteractType = InteractType.None;

        private static readonly Lazy<InteractHandler> _instance = new Lazy<InteractHandler>(() => new InteractHandler());

        public static InteractHandler Instance => _instance.Value;

        private InteractHandler()
        {
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DoInteract()
        {
            Debug.Log("Interact : " + currentInteractType);
        }

        public void SetInteractType(InteractType interactType)
        {
            currentInteractType = interactType;
        }
    }
}