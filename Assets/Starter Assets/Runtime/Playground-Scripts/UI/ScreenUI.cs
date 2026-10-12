using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace StarterAssets
{
    public class ScreenUI : MonoBehaviour
    {
        private PanelRenderer panelRenderer;
        private VisualElement rootVisualElement;
        private Label dialogueText;

        public static ScreenUI Instance { get; private set; }

        void Awake()
        {
            panelRenderer = GetComponent<PanelRenderer>();

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

        void OnEnable()
        {
            // Register the callback to handle initial load and live-reloads safely
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
        }

        void OnDisable()
        {
            // Always clean up your callbacks when disabled
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }

        private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
        {
            // Store a reference to the active root element
            rootVisualElement = root;

            // Query your elements safely now that the UI is guaranteed to be loaded
            dialogueText = rootVisualElement.Q<Label>("Dialogue");

            if (dialogueText != null)
            {
                dialogueText.visible = false;
            }
        }

        public IEnumerator ShowDialogue(string text)
        {
            if (dialogueText != null)
            {
                dialogueText.text = text;
                dialogueText.visible = true;
                Debug.Log("Showing dialogue: " + text);

                yield return new WaitForSeconds(2f);
                dialogueText.visible = false;
            }
        }
    }   
}