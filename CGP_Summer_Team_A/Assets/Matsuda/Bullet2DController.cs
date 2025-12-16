using UnityEngine;

public class Bullet2DController : MonoBehaviour
{
    public float rotationspeed=360f; 

    public enum ShooterType { Player, NPC }
    public ShooterType shooter = ShooterType.Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,rotationspeed*Time.deltaTime);
    }
}
