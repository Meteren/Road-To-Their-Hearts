using Cinemachine;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] Vector2 parallaxMultiplier;
    [SerializeField] Transform cameraTransform;
    Vector3 lastPositionOfCamera;
    float textureUnitSize;
    
   
    void Start()
    {
        GameManager.instance.mainCam.GetComponent<CinemachineConfiner>().m_BoundingShape2D =
               GameObject.Find("Background").transform.Find("6").GetComponent<PolygonCollider2D>();
        cameraTransform = Camera.main.transform;
        lastPositionOfCamera = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSize = texture.width / sprite.pixelsPerUnit;
        textureUnitSize = textureUnitSize * transform.localScale.x;
    }


    private void LateUpdate()
    {
        if(cameraTransform != null)
        {
            Vector3 deltaMovement = cameraTransform.position - lastPositionOfCamera;
            transform.position += new Vector3(deltaMovement.x * parallaxMultiplier.x, deltaMovement.y * parallaxMultiplier.y);
            lastPositionOfCamera = cameraTransform.position;

            if (cameraTransform.position.x - transform.position.x >= textureUnitSize)
            {
                float offset = (cameraTransform.position.x - transform.position.x) % textureUnitSize;
                transform.position = new Vector3(cameraTransform.position.x + offset, transform.position.y);
            }
        }
       
    }
}
