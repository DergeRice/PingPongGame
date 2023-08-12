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
    [SerializeField] GameObject MasterCanvas,ClientCanvas,CommonCanvas, Ball, RedBar,BlueBar, WaitOp, EnterOp, Panel;

    public bool ImBlue;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    public void ShowCanvas()
    {
        Panel.SetActive(false);
        if (Master) MasterCanvas.SetActive(true);
        else ClientCanvas.SetActive(true);
    }

    public void SetMyPlayerBlue(bool AmIblue)
    {
        if (AmIblue) ImBlue = true;
        else ImBlue = false;

        NetWorkManager.instance.ExitCanvas();

    }


    public void EndSetting()
    {
        Text CanvasText = CommonCanvas.transform.GetChild(0).GetComponent<Text>();

        if (Master)
        {
            MasterCanvas.SetActive(false);
            Ball.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
        }
        else ClientCanvas.SetActive(false);

        CommonCanvas.SetActive(true);
        if (ImBlue)
        {
            CanvasText.color = Color.blue;
            CanvasText.text = "Blue";
            BlueBar.GetComponent<BarScript>().Movable = true;

            if (Master)
            {
                SetBallPos(BlueBar);
                RedBar.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[1]); 
                // transfer ownership to other player (except me:0)
            }
        }
        else
        {
            CanvasText.text = "Red";
            CanvasText.color = Color.red;
            RedBar.GetComponent<BarScript>().Movable = true;
            if (Master)
            {
                SetBallPos(RedBar);
                RedBar.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[1]);
            }
        }
    }

    public void SetBallPos(GameObject Target)
    {
        Ball.transform.position =  Target.GetComponent<BarScript>().BallPos;
    }

    public void ResetGame()
    {
        RedBar.transform.position = RedBar.GetComponent<BarScript>().InitialPos;
        BlueBar.transform.position = BlueBar.GetComponent<BarScript>().InitialPos;
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
