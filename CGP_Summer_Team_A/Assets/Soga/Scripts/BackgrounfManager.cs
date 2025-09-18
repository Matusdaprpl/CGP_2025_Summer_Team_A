using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using UnityEngine.AI;

public class BackgroundManager : MonoBehaviour
{
    public List<Transform> backgrounds;

    private float backgroundWidth;
    private Camera mainCamera;
    private float cameraHalfWidth;

    private void Start()
    {
        mainCamera = Camera.main;
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        if (backgrounds != null && backgrounds.Count > 0)
        {
            backgroundWidth = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;
            backgrounds = backgrounds.OrderBy(bg => bg.position.x).ToList();

        }
    }

    private void LateUpdate()
    {
        if (backgrounds == null || backgrounds.Count == 0) return;
        Transform firstBackground = backgrounds[0];

        float cameraLeftEdge = mainCamera.transform.position.x - cameraHalfWidth;
        if(firstBackground.position.x+(backgroundWidth/2)<cameraLeftEdge)
        {
            Transform lastBackground = backgrounds[backgrounds.Count - 1];
            firstBackground.position = new Vector3(lastBackground.position.x + backgroundWidth, firstBackground.position.y, firstBackground.position.z);
            backgrounds.Remove(firstBackground);
            backgrounds.Add(firstBackground);
        }
    }
}