using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJumpHeightInput : MonoBehaviour
{
    private float m_JumpHeight;
    private InputField text;
    private GameObject m_Player;

    public void SetValues(float jumpHeight, GameObject player)
    {
        m_JumpHeight = jumpHeight;
        m_Player = player;

        text = GetComponent<InputField>();
        text.onValueChanged.AddListener(delegate {UpdatePlayerJumpHeight(); });
        text.text = "" + jumpHeight;
    }

    private void UpdatePlayerJumpHeight()
    {
        m_JumpHeight = float.Parse(text.text);
        m_Player.GetComponent<PlayerController>().SetJumpHeight(m_JumpHeight);
    }
}
