using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundHandler : MonoBehaviour
{
    // Variables

    // GameObjects
    public GameObject background;
    // GameObject accessors

    // Other files
    public Sprite loginBackground;
    public Sprite menuBackground;

    public void LoginBackground() 
    {
        // Changes background to login
        background.GetComponent<SpriteRenderer>().sprite = loginBackground;
    }
    
    public void MenuBackground()
    {
        // Changes background to menu
        background.GetComponent<SpriteRenderer>().sprite = menuBackground;
    }

    public void ShowBackground()
    {
        background.SetActive(true);
    }

    public void HideBackground()
    {
        background.SetActive(false);
    }
}
