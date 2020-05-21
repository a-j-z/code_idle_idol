using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportRecharge : MonoBehaviour
{
    private float fill;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }
    
    void Update()
    {
        image.fillAmount = fill;
    }

    public void SetFill(float fill)
    {
        this.fill = fill;
    }
}
