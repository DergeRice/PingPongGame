using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager instance;
    public bool DebugMode;
    public bool Master;

    [SerializeField] GameObject MasterCanvas,ClientCanvas,CommonCanvas,CountCanvas,InGameCanvas, RedBar,BlueBar, WaitOp, EnterOp, Panel, Timer;
    [SerializeField] GameObject ResultCanvas, BluePointText, RedPointText;

    public GameObject Ball, BallPrefeb;

    public bool ImBlue, GamePlaying;

    const string OpRunOutMent = "상대가 떠났습니다. 다음 상대를 기다리는 중"; 
    float GameTime;

    public int BluePoint, RedPoint;
    Vector3 BallInitialPos;
    [SerializeField] float LimitTime;

    public PhotonView PV;

    WaitForSeconds WaitOneSec = new WaitForSeconds(1f);
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        GameTime = LimitTime;
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        BallInitialPos = Ball.transform.position;
    }

    private void Update()
    {
        if(GamePlaying) SetTimer();
    }

    public void SetTimer()
    {
        GameTime -= Time.deltaTime;
        Timer.GetComponent<Text>().text = ((int)GameTime).ToString();

        if(GameTime < 0)
        {
            ShowResult();
        }
    }

    public void ShowResult()
    {
        ResultCanvas.SetActive(true);

        GameTime = LimitTime;
        Timer.GetComponent<Text>().text = ((int)GameTime).ToString();

        int MyPoint, OpPoint;
        if (ImBlue)
        {
            MyPoint = BluePoint;
            OpPoint = RedPoint;
        }else
        {
            MyPoint = RedPoint;
            OpPoint = BluePoint;
        }

        if (MyPoint > OpPoint)
        {
            ResultCanvas.transform.GetChild(0).GetComponent<Text>().text = "승리!!!";
        } else ResultCanvas.transform.GetChild(0).GetComponent<Text>().text = "패배..";

        ResetAll();


        EndSetting();

    }

    void ResetAll()
    {
        BluePoint = 0;
        RedPoint = 0;
        ResetPos();

        GamePlaying = false;
        RedBar.GetComponent<BarScript>().GamePlaying = false;
        BlueBar.GetComponent<BarScript>().GamePlaying = false;
    }

    public void ShowCanvas()
    {
        Panel.SetActive(false);
        if (Master) MasterCanvas.SetActive(true);
        else ClientCanvas.SetActive(true);
    }

    [PunRPC]
    public void SetMyPlayerBlue(bool AmIblue)
    {
        ImBlue = AmIblue;
        PV.RPC("EndSetting", RpcTarget.All);

    }

    public void BallHit(Vector3 Pos, bool ImBlue)
    {
        Destroy(Ball);
        GameObject NewBall = Instantiate(BallPrefeb);
        NewBall.transform.position = Pos;
        Ball = NewBall;

        if (ImBlue) RedPoint += 10;
        else BluePoint += 10;

        BluePointText.GetComponent<Text>().text = BluePoint.ToString();
        RedPointText.GetComponent<Text>().text = RedPoint.ToString();

        RedBar.GetComponent<BarScript>().Ball = NewBall;
        BlueBar.GetComponent<BarScript>().Ball = NewBall;
    }

    [PunRPC]
    public void EndSetting()
    {
        if (!GamePlaying)
        {
            GamePlaying = true;
            CountCanvas.SetActive(true);

            StartCoroutine("CountCanvasCoroutine", 5f);
            Invoke("StartGame", 5f);

            if (Master)
            {
                MasterCanvas.SetActive(false);
                Ball.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);

                if (ImBlue)
                {
                    PV.RPC("SetMyPlayerBlue", PhotonNetwork.PlayerList[1], false);

                    SetBallPos(BlueBar);
                    RedBar.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[1]);
                    BlueBar.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[0]);
                    // transfer ownership to other player (except me:0)
                    BlueBar.GetComponent<BarScript>().BallIsOnMyControl = true;

                }
                else
                {
                    PV.RPC("SetMyPlayerBlue", PhotonNetwork.PlayerList[1], true);

                    SetBallPos(RedBar);
                    RedBar.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[0]);
                    BlueBar.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[1]);
                    RedBar.GetComponent<BarScript>().BallIsOnMyControl = true;
                }
            }
            else ClientCanvas.SetActive(false);
        }

    }

    IEnumerator CountCanvasCoroutine(int InitialSec)
    {
        if (InitialSec == 0) 
        {
            RedBar.GetComponent<BarScript>().GamePlaying = true;
            BlueBar.GetComponent<BarScript>().GamePlaying = true;
            yield return null;
        }

        CountCanvas.transform.GetChild(0).GetComponent<Text>().text = $"{InitialSec}초 뒤에 게임을 시작합니다!!";
        yield return WaitOneSec;

        StartCoroutine("CountCanvasCoroutine",InitialSec - 1);
    }

    public void StartGame()
    {
        InGameCanvas.SetActive(true);
        CountCanvas.SetActive(false);
        GamePlaying = true;

        Text CanvasText = CommonCanvas.transform.GetChild(0).GetComponent<Text>();

        CommonCanvas.SetActive(true);
        if (ImBlue)
        {
            CanvasText.color = Color.blue;
            CanvasText.text = "Blue";
            BlueBar.GetComponent<BarScript>().Movable = true;

        }
        else
        {
            CanvasText.text = "Red";
            CanvasText.color = Color.red;
            RedBar.GetComponent<BarScript>().Movable = true;
        }
    }
    [PunRPC]
    public void SetBallPos(GameObject Target)
    {
        Ball.transform.position =  Target.GetComponent<BarScript>().BallPos;
        Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Ball.GetComponent<Rigidbody>().AddForce(Vector3.zero, ForceMode.VelocityChange);
    }


    public void ResetPos()
    {
        RedBar.transform.position = RedBar.GetComponent<BarScript>().InitialPos;
        BlueBar.transform.position = BlueBar.GetComponent<BarScript>().InitialPos;
        Ball.transform.position = BallInitialPos;
    }

    [PunRPC]
    public void OpRunOutGame()
    {
        ResetAll();
        ShowResult();
        MasterCanvas.SetActive(true);
        EnterOp.SetActive(false);
        WaitOp.SetActive(true);
        WaitOp.GetComponent<Text>().text = OpRunOutMent;
    }

    [PunRPC]
    public void EnteredOp()
    {
        WaitOp.SetActive(false);
        EnterOp.SetActive(true);
    }


    public void OnPhotonSerializeView(PhotonStream steam, PhotonMessageInfo info)
    {
        if (steam.IsWriting)
        {
            steam.SendNext(Ball.transform.position);
            steam.SendNext(RedBar.transform.position);
            steam.SendNext(BlueBar.transform.position);

            Ball.GetComponent<BallScript>().NetworkPos = Ball.transform.position;
            RedBar.GetComponent<BarScript>().NetworkPos = RedBar.transform.position;
            BlueBar.GetComponent<BarScript>().NetworkPos = BlueBar.transform.position;
        }
        else
        {
            Ball.GetComponent<BallScript>().NetworkPos = (Vector3)steam.ReceiveNext();
            RedBar.GetComponent<BarScript>().NetworkPos = (Vector3)steam.ReceiveNext();
            BlueBar.GetComponent<BarScript>().NetworkPos = (Vector3)steam.ReceiveNext();
        }
    }
}
