using UnityEngine;
using UnityEngine.UI;

public class ShowHintsCheckbox : MonoBehaviour
{
    public Toggle ToggleReference;

    private void Awake()
    {
    }
    private void Start()
    {
        ToggleReference.isOn = Settings.GetInstance().GetShowHints();
        ToggleReference.onValueChanged.AddListener(OnValueChanged);

    }
    private void OnDestroy()
    {
        ToggleReference.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(bool newValue)
    {
        Settings.GetInstance().SetShowHints(newValue);
    }
}
