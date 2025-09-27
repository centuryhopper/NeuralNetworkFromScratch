using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // new Input System

public class DrawingPad : MonoBehaviour
{
    public RawImage display; 
    public int size = 28; // MNIST size
    private Texture2D texture;


    void Awake()
    {
        display = GetComponent<RawImage>();
    }

    void Start()
    {
        texture = new Texture2D(size, size, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Point;
        Clear();
        display.texture = texture;
    }

    void Update()
    {
        // Left mouse pressed?
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue(); // ✅ new system

            if (RectTransformUtility.RectangleContainsScreenPoint(display.rectTransform, mousePos))
            {
                Vector2 localMouse;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    display.rectTransform, mousePos, null, out localMouse);

                Rect rect = display.rectTransform.rect;
                float px = (localMouse.x - rect.x) / rect.width;
                float py = (localMouse.y - rect.y) / rect.height;

                int x = Mathf.Clamp((int)(px * size), 0, size - 1);
                int y = Mathf.Clamp((int)(py * size), 0, size - 1);

                // black
                DrawPixel(x, y, 0f);

                // white
                // DrawPixel(x, y, 1f);
            }
        }
    }

    public void Clear()
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                texture.SetPixel(x, y, Color.white);
                // texture.SetPixel(x, y, Color.black);
            }
        }

        texture.Apply();
    }

    void DrawPixel(int x, int y, float value)
    {
        texture.SetPixel(x, y, new Color(value, value, value));
        texture.Apply();
    }

    public Texture2D GetTexture() => texture;
}
