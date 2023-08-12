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
    public bool Movable, BallIsOnMyControl;
    [SerializeField] float MoveSpeed, BallPower;

    public int OpPoint;
    public GameObject OpPointText,Ball;

    Vector3 MovVec;

    private void Awake()
    {

        //Movable = PV.IsMine ? true :false;
        PV = GetComponent<PhotonView>();
        InitialPos = transform.position;
    }
    private void Start()
    {
        Ball = GameManager.instance.Ball;
        BallPosObj = transform.GetChild(0).gameObject;
        BallPos = BallPosObj.transform.position;

        NetworkPos = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, NetworkPos, Time.deltaTime * 10);


        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (Movable)
            {
                transform.position -= Vector3.forward * Time.deltaTime * MoveSpeed;
              
            }
            if(GameManager.instance.Master)
            {
                BallControlAction(-Vector3.forward);
            }

        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (Movable)
            {
                transform.position += Vector3.forward * Time.deltaTime * MoveSpeed;
               
            }
            if (GameManager.instance.Master)
            {
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
            Ball.GetComponent<Rigidbody>().AddForce(Toward * BallPower, ForceMode.VelocityChange);
        }
    }

    public void Hit()
    {
        OpPoint += 10;
        OpPointText.GetComponent<Text>().text = OpPoint.ToString();
        BallPos = BallPosObj.transform.position;
        Ball.transform.position = BallPos;
        Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        BallIsOnMyControl = true;
    }
}
