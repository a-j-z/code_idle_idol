using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpeedInput : MonoBehaviour
{
    private float m_Speed;
    private InputField text;
    private GameObject m_Player;

    public void SetValues(float speed, GameObject player)
    {
        m_Speed = speed;
        m_Player = player;

        text = GetComponent<InputField>();
        text.onValueChanged.AddListener(delegate {UpdatePlayerSpeed(); });
        text.text = "" + speed;
    }

    private void UpdatePlayerSpeed()
    {
        m_Speed = float.Parse(text.text);
        m_Player.GetComponent<PlayerMovement>().SetSpeed(m_Speed);
    }
}
