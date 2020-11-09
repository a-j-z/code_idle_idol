using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButton : BaseButton
{
    private GameObject playerSpeedInput;
    private GameObject playerJumpHeightInput;

    public void SetPlayerValues(float speed, float jumpHeight, GameObject player)
    {
        playerSpeedInput = transform.GetChild(4).gameObject;
        playerSpeedInput.GetComponent<PlayerSpeedInput>().SetValues(speed, player);

        playerJumpHeightInput = transform.GetChild(5).gameObject;
        playerJumpHeightInput.GetComponent<PlayerJumpHeightInput>().SetValues(jumpHeight, player);
    }
}
