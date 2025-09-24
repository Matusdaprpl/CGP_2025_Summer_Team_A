using UnityEngine;

public class Bullet2DController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public float rotationspeed=360f;
    

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,rotationspeed*Time.deltaTime);
    }
}
