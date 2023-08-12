using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BarScript : MonoBehaviour
{
    [SerializeField] GameObject BallPosObj;
    public Vector3 BallPos, InitialPos, NetworkPos;

    public PhotonView PV;
    public bool Movable;
    [SerializeField] float MoveSpeed = 3;

    private void Awake()
    {
        //Movable = PV.IsMine ? true :false;

        InitialPos = transform.position;
    }
    private void Start()
    {
        BallPosObj = transform.GetChild(0).gameObject;
        BallPos = BallPosObj.transform.position;

        NetworkPos = transform.position;
    }

    private void Update()
    {
        if (Movable)
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.position -= Vector3.forward * Time.deltaTime * MoveSpeed;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.position += Vector3.forward * Time.deltaTime * MoveSpeed;
            }

            transform.position = Vector3.Lerp(transform.position, NetworkPos,Time.deltaTime*10);
        }
    }

    public void Hit()
    {

    }
}
