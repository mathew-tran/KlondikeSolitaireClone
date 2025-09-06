using TMPro;
using UnityEngine;

public class GamesWonText : MonoBehaviour
{
    public TMP_Text Text;
    public void Show()
    {
        gameObject.SetActive(true);
        Text.text = "Games Won\n" + PlayerPrefs.GetInt("GamesWon", 0);
    }
}
