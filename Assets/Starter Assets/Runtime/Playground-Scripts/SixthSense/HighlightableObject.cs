using UnityEngine;

namespace StarterAssets
{

    [RequireComponent(typeof(MeshRenderer))]
    public class HighlightableObject : MonoBehaviour
    {
        [Header("Reference")]
        private MeshRenderer meshRenderer;

        [Header("Highlight Settings")]
        public HighlightType highlightType = HighlightType.Interactable;
        public float outlineActiveWidth = 1.06f;
        private float outlineInactiveWidth = 1f;
        public bool pulseHighlight = true;
        private int materialIndex = 1;
        private Material outlineMaterial;

        [Header("Scent/Trail")]
        public bool leavesScentTrail = false;
        public float scentDuration = 10f;

        Renderer[] renderers;
        MaterialPropertyBlock propertyBlock;
        int outlineColorID;
        int outlineWidthID;
        int pulseID;
        int activeID;

        public enum HighlightType
        {
            Interactable,
            Enemy,
            Loot,
            Clue,
            Danger,
            Custom
        }

        void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            meshRenderer = GetComponent<MeshRenderer>();

            Material[] materials = meshRenderer.materials;
            outlineMaterial = materials[materialIndex];
            outlineMaterial.SetFloat("_Outline_Size", outlineInactiveWidth);

            propertyBlock = new MaterialPropertyBlock();

            outlineColorID = Shader.PropertyToID("_WitcherOutlineColor");
            outlineWidthID = Shader.PropertyToID("_WitcherOutlineWidth");
            pulseID = Shader.PropertyToID("_WitcherPulse");
            activeID = Shader.PropertyToID("_WitcherHighlightActive");
        }

        public void EnableHighlight(Color color)
        {
            outlineMaterial.SetFloat("_Outline_Size", outlineActiveWidth);
            outlineMaterial.SetColor("_Outline_Color", color);
        }

        public void DisableHighlight()
        {
            outlineMaterial.SetFloat("_Outline_Size", outlineInactiveWidth);
        }

        public void SetHighlightColor(Color color)
        {
            foreach (var rend in renderers)
            {
                rend.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(outlineColorID, color);
                rend.SetPropertyBlock(propertyBlock);
            }
        }
    }
}