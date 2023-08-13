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

    public GameObject Ball;

    public bool ImBlue, GamePlaying;

    float GameTime;

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
            GameTime = 0;
            FullResetGame();
            ShowResult();
        }
    }

    public void ShowResult()
    {

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
        NetWorkManager.instance.ExitCanvas();
    }

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

    public void SetBallPos(GameObject Target)
    {
        Ball.transform.position =  Target.GetComponent<BarScript>().BallPos;
        Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void ResetPos()
    {
        RedBar.transform.position = RedBar.GetComponent<BarScript>().InitialPos;
        BlueBar.transform.position = BlueBar.GetComponent<BarScript>().InitialPos;
    }

    public void FullResetGame()
    {
        RedBar.transform.position = RedBar.GetComponent<BarScript>().InitialPos;
        BlueBar.transform.position = BlueBar.GetComponent<BarScript>().InitialPos;
        Panel.SetActive(true);
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
