using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static BallManager instance;
    [SerializeField] private GameObject ball;
    private Vector3 pos;
    private Vector3 vel;
    private Vector3 direction;
    public byte lastTouchPlayerId;

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        pos = transform.position;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        vel = rb.velocity;
        direction = rb.transform.forward;
    }

    private void LateUpdate()
    {
        if (ServerInfoTransfer.Instance.connectionType == ConnectionType.HOST)
        {
            //Send data of ball to every player
            NetBallMessage message = new NetBallMessage(pos, vel, direction, lastTouchPlayerId);
            ServerManager.instance.BroadCast(message);
        }
    }

    public void UpdateBallPosition(Vector3 pos, Vector3 vel, Vector3 dir)
    {
        this.transform.position = pos;
        GetComponent<Rigidbody2D>().velocity = vel;
        this.transform.forward = dir;
    }

    public void SetTouchId(byte id)
    {
        lastTouchPlayerId = id;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<HoopManager>() != null) {
            Debug.Log("Triggered a Hoop");
            Score();
        }
    }

    private void Score()
    {
        //Return if the touched is not the host (their client will send the info)
        if (lastTouchPlayerId != ClientManager.Instance.playerId) { return; }
        PlayerListManager plm = PlayerListManager.Instance;
        byte newScore = (byte)(plm.getPlayerScore(lastTouchPlayerId) + 1);

        NetUpdateScore message = new NetUpdateScore(lastTouchPlayerId, newScore);
        ClientManager.Instance.SendtoServer(message);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerListManager plm = PlayerListManager.Instance;
            lastTouchPlayerId = plm.getPlayerId(collision.gameObject);
            Debug.Log("Tocuhed player id = " + lastTouchPlayerId);
        }
    }

    public void ResetBall()
    {
        ball.transform.position = Vector3.zero;
        ball.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        ball.transform.forward = Vector3.zero;
        NetBallMessage message = new NetBallMessage(pos, vel, direction, lastTouchPlayerId);
        ServerManager.instance.BroadCast(message);
    }
}
