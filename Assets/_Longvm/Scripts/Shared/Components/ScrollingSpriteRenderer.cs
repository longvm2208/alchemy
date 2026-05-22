using UnityEngine;

public class ScrollingSpriteRenderer : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Vector2 speed = Vector2.one;

    private void Start()
    {
        spriteRenderer.material = new Material(spriteRenderer.material);
    }

    private void Update()
    {
        spriteRenderer.material.mainTextureOffset += speed * Time.deltaTime;
    }

    public void SetSpeed(Vector2 newSpeed)
    {
        speed = newSpeed;
    }
}
