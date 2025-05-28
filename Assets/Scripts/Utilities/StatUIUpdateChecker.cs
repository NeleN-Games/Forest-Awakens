using UnityEngine;

namespace Utilities.UI
{
    public static class StatUIUpdateChecker
    {
        public static bool ShouldUpdateUI(float currentValue, float newValue, float maxValue, int thresholdPercent = 1)
        {
            var differenceValue = Mathf.Abs(currentValue - newValue);
            var differencePercentage = differenceValue / maxValue * 100f;
            return differencePercentage > thresholdPercent;
        }
    }
}