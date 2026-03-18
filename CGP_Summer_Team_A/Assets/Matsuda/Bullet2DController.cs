using UnityEngine;

public class Bullet2DController : MonoBehaviour
{
    public float rotationspeed=360f; 

    public enum ShooterType { Player, NPC }
    public ShooterType shooter = ShooterType.Player;
    
    //public PlayerMove shooterPlayer;
    //public NPCplayer shooterNpc;
    //public int transferPoints = 1000;
    void Update()
    {
        transform.Rotate(0,0,rotationspeed*Time.deltaTime);
    }
}
