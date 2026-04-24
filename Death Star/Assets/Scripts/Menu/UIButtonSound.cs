using UnityEngine;

public class UIButtonSound : MonoBehaviour
{
    public void PlayClick()
    {
        if (AudioManager.instancia != null)
        {
            AudioManager.instancia.PlayClick();
        }
    }
}