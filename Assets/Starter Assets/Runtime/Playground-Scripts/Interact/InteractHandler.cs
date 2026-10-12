using System;
using UnityEngine;

namespace StarterAssets
{
    public class InteractHandler : MonoBehaviour
    {
        public static InteractHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                Instance = this;
            }
        }

        public enum InteractType
        {
            None,
            Dialogue,
            Quest,
            Shop
        }

        private InteractType currentInteractType = InteractType.None;

        private ScreenUI screenUI;

        private void Start()
        {
            screenUI = ScreenUI.Instance;
        }

        public void DoInteract()
        {
            switch (currentInteractType)
            {
                case InteractType.Quest:
                    //Quest
                    break;
                case InteractType.Dialogue:
                    StartCoroutine(screenUI.ShowDialogue("Howdy!"));
                    break;
                default:
                    Debug.Log("No interaction available.");
                    break;
            }
        }

        public void SetInteractType(InteractType interactType)
        {
            currentInteractType = interactType;
        }
    }
}