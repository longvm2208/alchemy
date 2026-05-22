using UnityEngine;
using UnityEngine.Tilemaps;

public static class RendererExtensions
{
    public static void SetAlpha(this SpriteRenderer sr, float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }

    public static void SetSizeX(this SpriteRenderer sr, float sizeX)
    {
        sr.size = new Vector2(sizeX, sr.size.y);
    }

    public static void SetSizeY(this SpriteRenderer sr, float sizeY)
    {
        sr.size = new Vector2(sr.size.x, sizeY);
    }

    public static void SetAlpha(this Tilemap tilemap, float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        Color color = tilemap.color;
        color.a = alpha;
        tilemap.color = color;
    }
}
