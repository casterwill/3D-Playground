using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace StarterAssets
{
    public class SixthSense : MonoBehaviour
    {
        public static SixthSense Instance { get; private set; }

        [Header("Settings")]
        public float activationTime = 0.3f;
        public float cooldownTime = 1f;
        public LayerMask highlightLayers;
        public float highlightRange = 30f;

        [Header("Visual Effects")]
        public Volume senseVolume;

        public Color highlightColor = Color.yellow;
        public Color scentColor = new Color(1f, 0.5f, 0f);
        public float pulseSpeed = 2f;

        [Header("Audio")]
        public AudioClip activateSound;
        public AudioClip deactivateSound;
        public AudioClip pulseSound;
        public float pulseInterval = 3f;

        // State
        bool isActive = false;
        float cooldownTimer = 0f;
        float pulseTimer = 0f;
        AudioSource audioSource;

        // Post-processing
        Vignette vignette;
        ColorAdjustments colorAdjustments;
        Bloom bloom;

        // Highlighted objects
        private List<HighlightableObject> highlightedObjects = new();

        void Awake()
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            SetupPostProcessing();
        }

        void SetupPostProcessing()
        {
            if (senseVolume != null)
            {
                senseVolume.profile.TryGet(out vignette);
                senseVolume.profile.TryGet(out colorAdjustments);
                senseVolume.profile.TryGet(out bloom);
            }
        }

        void Update()
        {
            //HandleInput();
            HandleCooldown();

            if (isActive)
            {
                UpdatePulse();
                ScanForObjects();
            }
        }

        //public void OnSixthSense(InputValue value)
        //{
        //    if (value.isPressed && cooldownTimer <= 0f)
        //    {
        //        Debug.Log("Sixth Sense Input Detected");
        //        ToggleSense();
        //    }
        //}

        void HandleInput()
        {

        }

        public void ToggleSense()
        {
            //if (cooldownTimer > 0f) return;

            isActive = !isActive;
            cooldownTimer = cooldownTime;

            if (isActive) ActivateSense();
            else DeactivateSense();
        }

        void ActivateSense()
        {
            // Audio
            if (activateSound) audioSource.PlayOneShot(activateSound);

            // Visual transition
            StartCoroutine(TransitionVignette(0f, 0.45f, activationTime));
            StartCoroutine(TransitionSaturation(0f, -80f, activationTime));
            StartCoroutine(TransitionBloom(0f, 1.5f, activationTime));

            // Pulse timer
            pulseTimer = 0f;

            // Initial scan
            ScanForObjects();
        }

        void DeactivateSense()
        {
            if (deactivateSound) audioSource.PlayOneShot(deactivateSound);

            StartCoroutine(TransitionVignette(0.45f, 0f, activationTime));
            StartCoroutine(TransitionSaturation(-80f, 0f, activationTime));
            StartCoroutine(TransitionBloom(1.5f, 0f, activationTime));

            ClearHighlights();
        }

        void UpdatePulse()
        {
            pulseTimer += Time.deltaTime;

            // Periodic pulse effect
            if (pulseTimer >= pulseInterval)
            {
                pulseTimer = 0f;
                TriggerPulse();
            }

            // Continuous pulse animation for highlighted objects
            float pulseValue = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            Shader.SetGlobalFloat("_WitcherSensePulse", pulseValue);
            Shader.SetGlobalFloat("_WitcherSenseActive", 1f);
        }

        void TriggerPulse()
        {
            if (pulseSound) audioSource.PlayOneShot(pulseSound, 0.5f);

            // Visual pulse - expand ring shader effect
            Shader.SetGlobalVector("_WitcherSensePulseOrigin", transform.position);
            Shader.SetGlobalFloat("_WitcherSensePulseTime", Time.time);
        }

        void ScanForObjects()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, highlightRange, highlightLayers);

            foreach (var hit in hits)
            {
                var highlightable = hit.GetComponent<HighlightableObject>();
                if (highlightable != null && !highlightedObjects.Contains(highlightable))
                {
                    highlightedObjects.Add(highlightable);
                    highlightable.EnableHighlight(highlightColor);
                }
            }

            // Remove out-of-range objects
            highlightedObjects.RemoveAll(obj =>
            {
                if (obj == null || Vector3.Distance(transform.position, obj.transform.position) > highlightRange)
                {
                    if (obj != null) obj.DisableHighlight();
                    return true;
                }
                return false;
            });
        }

        void ClearHighlights()
        {
            foreach (var obj in highlightedObjects)
            {
                if (obj != null) obj.DisableHighlight();
            }
            highlightedObjects.Clear();
            Shader.SetGlobalFloat("_WitcherSenseActive", 0f);
        }

        void HandleCooldown()
        {
            if (cooldownTimer > 0f)
                cooldownTimer -= Time.deltaTime;
        }

        // Coroutine helpers for smooth transitions
        IEnumerator TransitionVignette(float from, float to, float duration)
        {
            if (vignette == null) yield break;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                vignette.intensity.value = Mathf.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            vignette.intensity.value = to;
        }

        IEnumerator TransitionSaturation(float from, float to, float duration)
        {
            if (colorAdjustments == null) yield break;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            colorAdjustments.saturation.value = to;
        }

        IEnumerator TransitionBloom(float from, float to, float duration)
        {
            if (bloom == null) yield break;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                bloom.intensity.value = Mathf.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            bloom.intensity.value = to;
        }

        // Public API for scent trails, clues, etc.
        //public void RegisterScentTrail(ScentTrail trail) { /* ... */ }
        //public void RegisterClue(Clue clue) { /* ... */ }
    }
}