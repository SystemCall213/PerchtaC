using Oneiromancer.TMP.Effects;
using TMPro;
using UnityEngine;

namespace DefaultNamespace.Dialogue.Text
{
    [System.Serializable]
    public class ShiveringText : BaseTextEffect
    {
        public override string Tag => "shiver";

        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _amplitude = 2f;

        protected override void ApplyToCharacter(TMP_Text text, TMP_CharacterInfo charInfo)
        {
            if (!charInfo.isVisible) return;

            int materialIndex = charInfo.materialReferenceIndex;
            Vector3[] newVertices = text.textInfo.meshInfo[materialIndex].vertices;

            float time = Time.realtimeSinceStartup * _speed;
            
            // Shiver only up and down (Y-axis)
            // Use a large multiplier for index to make neighboring letters have very different noise values
            float yOffset = (Mathf.PerlinNoise(charInfo.index * 10.0f, time) - 0.5f) * _amplitude;

            Vector3 offset = new Vector3(0, yOffset, 0);

            for (int i = 0; i < 4; i++)
            {
                newVertices[charInfo.vertexIndex + i] += offset;
            }
        }
    }
}