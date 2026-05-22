using UnityEngine;
using UnityEngine.UI;

public class ScrollingImage : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Vector2 speed = Vector2.one;

    private void Start()
    {
        image.material = new Material(image.material);
    }

    private void Update()
    {
        image.material.mainTextureOffset += speed * Time.deltaTime;
    }

    public void SetSpeed(Vector2 newSpeed)
    {
        speed = newSpeed;
    }
}
