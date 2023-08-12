using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BallScript : MonoBehaviourPunCallbacks
{
    [SerializeField] bool DebugMode;
    GameManager gameManager;
    [SerializeField] const int PowerRangeMin = 20;
    [SerializeField] const int PowerRangeMax = 30;
    PhotonView PV;


    public Vector3 NetworkPos, InitialPos;

    Rigidbody rigid;
    // Start is called before the first frame update

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        gameManager = GameManager.instance;
        InitialPos = transform.position;
        NetworkPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
       if(gameManager.DebugMode) DebugModeAction();

        transform.position = Vector3.Lerp(transform.position, NetworkPos, Time.deltaTime * 10);
    }

    void DebugModeAction()
    {
        if (PV.IsMine && Input.GetKeyDown(KeyCode.F))
        {
            rigid.AddForce(new Vector3(Random.Range(PowerRangeMin, PowerRangeMax), Random.Range(PowerRangeMin, PowerRangeMax), Random.Range(PowerRangeMin, PowerRangeMax)),ForceMode.VelocityChange);
        }
    }
}
