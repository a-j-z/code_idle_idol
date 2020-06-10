using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    public GameObject player;
    public GameObject idol;
    public SpawnController spawn;

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

    public void Play(Vector3 spawnDirection)
    {
        buildCam.enabled = false;
        playCam.enabled = true;

        player.SetActive(true);
        idol.SetActive(true);

        cursor.SetActive(false);
        buildUiContainer.SetActive(false);
        playUiContainer.SetActive(true);

        GameObject spawnPoint = spawn.GetSpawn(spawnDirection);
        Vector3 spawnLocation = spawnPoint.activeSelf ? spawnPoint.transform.position : Vector3.zero;
        player.GetComponent<Rigidbody2D>().position = spawnLocation;
        player.GetComponent<PlayerController>().Init(spawnDirection);
        idol.GetComponent<Rigidbody2D>().position = spawnLocation;
        playCam.GetComponent<PlayCameraController>().GoToDestination(player.GetComponent<Rigidbody2D>().position);
        spawn.gameObject.SetActive(false);

        isPlay = true;
    }

    private void Build()
    {
        buildCam.enabled = true;
        playCam.enabled = false;
        player.SetActive(false);
        idol.SetActive(false);
        
        cursor.SetActive(true);
        buildUiContainer.SetActive(true);
        playUiContainer.SetActive(false);

        spawn.gameObject.SetActive(true);
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
            Play(Vector3.left);
        }
    }

    public static bool GetIsPlay()
    {
        return isPlay;
    }
}
