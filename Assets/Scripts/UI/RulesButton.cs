using UnityEngine;

public class RulesButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OpenRulesLink()
    {
        
    }

    public void OpenWikiLink()
    {
        Application.OpenURL("https://en.wikipedia.org/wiki/Klondike_(solitaire)");
   
    }
}
