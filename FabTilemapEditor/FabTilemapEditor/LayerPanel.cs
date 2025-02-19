using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

public class LayerPanel(Rectangle rectangle, string name, int index, Action onClick, Texture2D gearIcon)
{
    private bool isActive = false;

    public Rectangle Rect { get; set; } = rectangle;
    public int Index { get; set; } = index;
    public string Name { get; private set; } = name;

    public void ToggleActive() => isActive = !isActive;

    public void Update()
    {
        Vector2 mousePos = Raylib.GetMousePosition();

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            if (Raylib.CheckCollisionPointRec(mousePos, Rect))
                onClick?.Invoke();
    }

    public void Draw()
    {
        var color = isActive ? Color.LightGray : Color.DarkGray;
        Raylib.DrawRectangleRec(Rect, color);
        Raylib.DrawRectangleLinesEx(Rect, 1, Color.LightGray);

        // Centered text
        int textX = (int)Rect.X + 40;
        int textY = (int)(Rect.Y + (Rect.Height / 2) - 8);
        Raylib.DrawText(Name, textX, textY, 16, Color.White);

        // Draw Gear Icon
        Raylib.DrawTexture(gearIcon, (int)Rect.X + (int)Rect.Width - 32, (int)Rect.Y + 2, Color.DarkGray);
    }
}

