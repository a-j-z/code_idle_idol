using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyButton : BaseButton
{
    private GameObject basicEnemySpeedInput;

    public void SetBasicEnemyValues(float speed, GameObject basicEnemy)
    {
        basicEnemySpeedInput = transform.GetChild(4).gameObject;
        basicEnemySpeedInput.GetComponent<BasicEnemySpeedInput>().SetValues(speed, basicEnemy);
    }
}
