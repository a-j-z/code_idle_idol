using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    public GameObject player;
    public GameObject spawn;

    public Camera buildCam;
    public Camera playCam;

    public GameObject cursor;
    public GameObject buildUiContainer;
    public GameObject playUiContainer;

    private static bool isPlay;

    void Start()
    {
        Build();
    }

    private void Play()
    {
        buildCam.enabled = false;
        playCam.enabled = true;
        player.SetActive(true);

        cursor.SetActive(false);
        buildUiContainer.SetActive(false);
        playUiContainer.SetActive(true);

        player.transform.position = spawn.transform.position;
        spawn.SetActive(false);
        isPlay = true;
    }

    private void Build()
    {
        buildCam.enabled = true;
        playCam.enabled = false;
        player.SetActive(false);
        
        cursor.SetActive(true);
        buildUiContainer.SetActive(true);
        playUiContainer.SetActive(false);

        spawn.SetActive(true);
        isPlay = false;
    }

    public void SwitchMode()
    {
        if (isPlay)
        {
            Build();
        }
        else
        {
            Play();
        }
    }

    public static bool GetIsPlay()
    {
        return isPlay;
    }
}
