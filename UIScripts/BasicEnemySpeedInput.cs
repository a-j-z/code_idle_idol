using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicEnemySpeedInput : MonoBehaviour
{
    private float m_Speed;
    private InputField text;
    private GameObject m_BasicEnemy;

    public void SetValues(float speed, GameObject basicEnemy)
    {
        m_Speed = speed;
        m_BasicEnemy = basicEnemy;

        text = GetComponent<InputField>();
        text.onValueChanged.AddListener(delegate {UpdateBasicEnemySpeed(); });
        text.text = "" + speed;
    }

    private void UpdateBasicEnemySpeed()
    {
        m_Speed = float.Parse(text.text);
        m_BasicEnemy.GetComponent<NodeLineDraw>().SetSpeed(m_Speed);
    }
}
