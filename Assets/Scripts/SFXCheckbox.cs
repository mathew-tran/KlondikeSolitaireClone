using UnityEngine;
using UnityEngine.UI;

public class SFXCheckbox : MonoBehaviour
{
    public Toggle ToggleReference;

    private void Awake()
    {

    }
    private void Start()
    {
        ToggleReference.isOn = Settings.GetInstance().GetPlaySFX();
        ToggleReference.onValueChanged.AddListener(OnValueChanged);
    }
    private void OnDestroy()
    {
        ToggleReference.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(bool newValue)
    {
        Settings.GetInstance().SetPlaySFX(newValue);
    }
}
