using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class BarScript : MonoBehaviour
{
    [SerializeField] GameObject BallPosObj;
    public Vector3 BallPos, InitialPos, NetworkPos;

    public PhotonView PV;
    public bool Movable, BallIsOnMyControl, GamePlaying, ImBlue;
    [SerializeField] float MoveSpeed, BallPower;

    
    public GameObject OpPointText,Ball;

    Vector3 MovVec;

    private void Awake()
    {

        //Movable = PV.IsMine ? true :false;
        PV = GetComponent<PhotonView>();
        InitialPos = transform.position;

        BallPosObj = transform.GetChild(0).gameObject;
        BallPos = BallPosObj.transform.position;
    }
    private void Start()
    {
        Ball = GameManager.instance.Ball;
        

        NetworkPos = transform.position;
    }

    private void Update()
    {
        //transform.position = Vector3.Lerp(transform.position, NetworkPos, Time.deltaTime * 10);

        if (Movable && GamePlaying)
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.position -= Vector3.forward * Time.deltaTime * MoveSpeed;
                BallControlAction(-Vector3.forward);
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.position += Vector3.forward * Time.deltaTime * MoveSpeed;
                BallControlAction(Vector3.forward);
            }
        }
        

        
    }

    public void BallControlAction(Vector3 Toward)
    {
        if (BallIsOnMyControl)
        {
            BallIsOnMyControl = false;
            if (GameManager.instance.ImBlue) Toward += Vector3.left;
            else Toward += Vector3.right;

            PV.RPC("BallShoot", PhotonNetwork.MasterClient, Toward);
           
        }
    }

    [PunRPC]
    public void BallShoot(Vector3 Toward)
    {
        Ball.GetComponent<Rigidbody>().AddForce(Toward * BallPower, ForceMode.VelocityChange);
    }

    [PunRPC]
    public void Hit()
    {
        Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Ball.GetComponent<Rigidbody>().AddForce(Vector3.zero, ForceMode.VelocityChange);


        BallPos = BallPosObj.transform.position;
        Ball.transform.position = BallPos;

        GameManager.instance.BallHit(BallPos,ImBlue);

        BallIsOnMyControl = true;
    }

    void SetPosBack()
    {
        
    }
}
