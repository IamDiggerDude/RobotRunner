using UnityEngine;

public class Skin : MonoBehaviour
{
    [SerializeField]
    GameObject NormalSkin;
    [SerializeField]
    GameObject PowerUpSkin;

    public void ShowNormalSkin(bool show)
    {
        NormalSkin.SetActive(show);
        PowerUpSkin.SetActive(!show);
    }
}