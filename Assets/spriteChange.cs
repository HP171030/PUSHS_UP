using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class spriteChange : MonoBehaviour
{
    public Image img;
    public Sprite after_img;

    void Start()
    {
        img = GetComponent<Image>();
    }

    public void ChangeImage()
    {
        img.sprite = after_img;
    }
}