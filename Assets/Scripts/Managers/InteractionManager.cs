
using TMPro;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI toolTipText;
    
    public static InteractionManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        HideTooltip();
    }
    public void ShowTooltip(string text, Vector3 position)
    {
        toolTipText.gameObject.SetActive(true);
        toolTipText.transform.position = position;
        toolTipText.text = text;
    }

    public void HideTooltip()
    {
        toolTipText.gameObject.SetActive(false);
    }
}
