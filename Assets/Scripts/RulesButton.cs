using UnityEngine;

public class RulesButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OpenRulesLink()
    {
        Application.OpenURL("https://bicyclecards.com/how-to-play/klondike");
    }
}
