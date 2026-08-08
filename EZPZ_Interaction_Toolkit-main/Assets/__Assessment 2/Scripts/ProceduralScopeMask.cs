using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class ProceduralScopeMask : MonoBehaviour
{
    [Range(0.1f, 0.48f)]
    public float holeRadiusRatio = 0.38f; 
    public float featherWidth = 8f;        

    private void OnEnable()
    {
        GenerateMask();
    }

    private void OnValidate() 
    {
        GenerateMask();
    }

    private void GenerateMask()
    {
        Image img = GetComponent<Image>();
        if (img == null) return;

        int res = 1024;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(res / 2f, res / 2f);
        float radius = res * holeRadiusRatio;

        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01((dist - radius) / featherWidth);
                tex.SetPixel(x, y, new Color(0f, 0f, 0f, alpha));
            }
        }
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        img.sprite = sprite;
    }
}
