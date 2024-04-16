using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChage : MonoBehaviour
{
    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] GameObject Score;
    [SerializeField] GameObject Mirror;
    [SerializeField] GameObject Bomb;

    [ContextMenu("Test")]
    public void ChangCharater()
    {
        if (!player1.activeSelf)
        {
            Debug.Log("食切 -> 害切");
            player1.SetActive(true);
            Mirror.SetActive(true);
            player2.SetActive(false);
            Score.SetActive(false);
            Bomb.SetActive(false);
        }
        else
        {
            Debug.Log("害切 -> 食切");
            player2.SetActive(true);
            Mirror.SetActive(false);
            player1.SetActive(false);
            Score.SetActive(true);
            Bomb.SetActive(true);
        }

    }
}
