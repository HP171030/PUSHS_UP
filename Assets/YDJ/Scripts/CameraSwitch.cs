using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] public CinemachineVirtualCamera player1Camera;
    [SerializeField] public CinemachineVirtualCamera player2Camera;
    //[SerializeField] GameObject player1;
    //[SerializeField] GameObject player2;
    //[SerializeField] GameObject Mirror;
    //[SerializeField] GameObject Bomb;
    private GameObject rings;
    private GameObject bombCount;

    public UnityEvent OnChangePlayer;

    [SerializeField] bool isPlayer1Active;
    public bool IsPlayer1Active { get { return isPlayer1Active; } set { isPlayer1Active = value; } }

    void Start()
    {


        //player1 = rings.transform.Find("Player1Image");
        //player2 = GameObject.Find("Bomb");
        //Mirror = GameObject.Find("MirrorImage");
        //Bomb = GameObject.Find("Bomb");



        if (player1Camera.Follow == null)
        {
            Change();
            Manager.game.isEnter = true;
        }
        else
        {

        }
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1);
        rings = GameObject.Find("Rings");
        bombCount = GameObject.Find("BombCount");
    }

    private void OnChange(InputValue value)
    {
        if (!Manager.game.isEnter)
        {
            Change();
        }
        else
        {
            Debug.Log("isNotChange");
        }
    }

    public void Change()
    {
        if (isPlayer1Active && player2Camera.Follow != null)
        {
            Debug.Log(player1Camera.Priority);
            Debug.Log("p2On");
            OnChangePlayer?.Invoke();
            player1Camera.Priority = 0;
            player2Camera.Priority = 10;

            isPlayer1Active = false;

            rings.transform.Find("Player1Image").gameObject.SetActive(false);
            rings.transform.Find("Player2Image").gameObject.SetActive(true);

            bombCount.transform.Find("MirrorImage").gameObject.SetActive(false);
            bombCount.transform.Find("Bomb").gameObject.SetActive(true);

            //player2.SetActive(true);
            //Mirror.SetActive(false);
            //player1.SetActive(false);
            //Bomb.SetActive(true);

        }
        // �׷��� ������ �÷��̾� 1�� ī�޶�� ��ȯ�մϴ�.
        else if (!isPlayer1Active)
        {
            Debug.Log("p1On");
            OnChangePlayer?.Invoke();
            player1Camera.Priority = 10;
            Debug.Log(player1Camera.Priority);
            Debug.Log(player1Camera.name);


            rings.transform.Find("Player1Image").gameObject.SetActive(true);
            rings.transform.Find("Player2Image").gameObject.SetActive(false);

            bombCount.transform.Find("MirrorImage").gameObject.SetActive(true);
            bombCount.transform.Find("Bomb").gameObject.SetActive(false);

            //player1.SetActive(true);
            //Mirror.SetActive(true);
            //player2.SetActive(false);
            //Bomb.SetActive(false);

            while (player2Camera.Priority == 10)
            {
                Debug.Log("�� �ȹٲ��");
                player2Camera.Priority = 0;
            }

            isPlayer1Active = true;
            Debug.Log("�� �ȹٲ��1");
            Debug.Log("�� �ȹٲ��2");
            Debug.Log("�� �ȹٲ��3");
            Debug.Log("�� �ȹٲ��4");
            Debug.Log("�� �ȹٲ��5");
            Debug.Log("�� �ȹٲ��6");
            Debug.Log("�� �ȹٲ��7");
        }
        else
        {
            Debug.Log("?");
        }


    }
}
