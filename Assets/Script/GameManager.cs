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

    public bool ImBlue, GamePlaying, TimerGo;

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
        Screen.SetResolution(640, 360,false);
        instance = this;
        GameTime = LimitTime;
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        //BallInitialPos = Ball.transform.position;
    }

    private void Update()
    {
        if(GamePlaying&&TimerGo) SetTimer();
    }

    public void SetTimer()
    {
        GameTime -= Time.deltaTime;
        Timer.GetComponent<Text>().text = ((int)GameTime).ToString();

        if(GameTime < 0)
        {
            ShowResult(true);
        }
    }

    public void ShowResult(bool ReStart)
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

        if(MyPoint ==  OpPoint)
        {
            ResultCanvas.transform.GetChild(0).GetComponent<Text>().text = "무승부";
        }
        else if (MyPoint > OpPoint)
        {
            ResultCanvas.transform.GetChild(0).GetComponent<Text>().text = "승리!!!";
        } else ResultCanvas.transform.GetChild(0).GetComponent<Text>().text = "패배..";

        ResetAll();

        if(ReStart) EndSetting();


    }

    void ResetAll()
    {
        BluePoint = 0;
        RedPoint = 0;

        ResetPos();
        
        TimerGo = false;
        GamePlaying = false;
        RedBar.GetComponent<BarScript>().GamePlaying = false;
        BlueBar.GetComponent<BarScript>().GamePlaying = false;


        BluePointText.GetComponent<Text>().text = BluePoint.ToString();
        RedPointText.GetComponent<Text>().text = RedPoint.ToString();
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
        //Destroy(Ball);
        //GameObject NewBall = Instantiate(BallPrefeb);
        //NewBall.transform.position = Pos;
        Ball.transform.position = Pos;
        //Ball = NewBall;

        if (ImBlue) RedPoint += 10;
        else BluePoint += 10;

        BluePointText.GetComponent<Text>().text = BluePoint.ToString();
        RedPointText.GetComponent<Text>().text = RedPoint.ToString();

       // RedBar.GetComponent<BarScript>().Ball = NewBall;
        //BlueBar.GetComponent<BarScript>().Ball = NewBall;
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
                Player ClientPlayer,MasterClient;

                ClientPlayer = null;
                MasterClient = null;

                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    if (PhotonNetwork.PlayerList[i].IsMasterClient) MasterClient = PhotonNetwork.PlayerList[i];
                    else ClientPlayer = PhotonNetwork.PlayerList[i];
                }
                
                MasterCanvas.SetActive(false);
                Ball.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);

                if (ImBlue)
                {
                    PV.RPC("SetMyPlayerBlue", ClientPlayer, false);

                    SetBallPos(BlueBar);
                    RedBar.GetComponent<PhotonView>().TransferOwnership(ClientPlayer);
                    BlueBar.GetComponent<PhotonView>().TransferOwnership(MasterClient);
                    // transfer ownership to other player (except me:0)
                    

                }
                else
                {
                    PV.RPC("SetMyPlayerBlue", ClientPlayer, true);

                    SetBallPos(RedBar);
                    RedBar.GetComponent<PhotonView>().TransferOwnership(MasterClient);
                    BlueBar.GetComponent<PhotonView>().TransferOwnership(ClientPlayer);
                }
            }
            else ClientCanvas.SetActive(false);
        }

    }

    IEnumerator CountCanvasCoroutine(int InitialSec)
    {
        if (InitialSec <= 0) 
        {
            RedBar.GetComponent<BarScript>().GamePlaying = true;
            BlueBar.GetComponent<BarScript>().GamePlaying = true;
            ResultCanvas.SetActive(false);
            StopCoroutine(nameof(CountCanvasCoroutine));
            TimerGo = true;
            yield return null;
            
        }

        CountCanvas.transform.GetChild(0).GetComponent<Text>().text = $"{InitialSec}초 뒤에 게임을 시작합니다!!";

        if (InitialSec <= 0) { yield return null; }
        else {
            yield return WaitOneSec;
            StartCoroutine("CountCanvasCoroutine", InitialSec - 1);
        }

        
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
        Target.GetComponent<BarScript>().BallIsOnMyControl = true;
    }


    public void ResetPos()
    {
        RedBar.transform.position = RedBar.GetComponent<BarScript>().InitialPos;
        BlueBar.transform.position = BlueBar.GetComponent<BarScript>().InitialPos;
        Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        Ball.transform.position = Ball.GetComponent<BallScript>().InitialPos;
    }

    [PunRPC]
    public void OpRunOutGame()
    {
        ResetAll();
        ShowResult(false);
        InGameCanvas.SetActive(false);
        CommonCanvas.SetActive(false);
        MasterCanvas.SetActive(true);


        RedBar.GetComponent<BarScript>().Movable = false;
        BlueBar.GetComponent<BarScript>().Movable = false;

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
