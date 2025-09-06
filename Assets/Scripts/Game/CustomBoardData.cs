using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomBoardData", menuName = "Content/CustomBoardData")]
public class CustomBoardData : ScriptableObject
{
    public Material BoardMaterial;
    public string BoardName;
}
