using Arctic.World;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arctic.UI.Player
{
    public sealed class PlayerTemperatureGuage : MonoBehaviour
    {
        private const float UPDATE_INTERVAL_SEC = 0.04f;

        [SerializeField]
        private TextMeshProUGUI tempText;
        [SerializeField]
        private ThermalReactor playerThermalReactor;
        [SerializeField]
        private Image guageImage;
        [SerializeField]
        private Vector2 guageFillRange = new Vector2(-100, 100f);

        private void Start()
        {
            UpdateTemp();
            InvokeRepeating(nameof(UpdateTemp), 0.1f, UPDATE_INTERVAL_SEC);
        }

        private void UpdateTemp()
        {
            float effectiveTemp = playerThermalReactor.GetEffectiveTempF();
            tempText.text = $"{effectiveTemp:0.0}°F";
            float fill = 1 - Mathf.InverseLerp(guageFillRange.x, guageFillRange.y, effectiveTemp) * 0.5f;
            guageImage.fillAmount = fill;
        }
    }
}
