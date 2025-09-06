using TMPro;
using UnityEngine;

public class SeedNumberText : MonoBehaviour
{
    [SerializeField]
    TMP_Text Text;

    [SerializeField]
    private GameManager ManagerReference;

    private void Start()
    {
        Text.text = "Seed:" + ManagerReference.GetSeed().ToString();


    }
}
