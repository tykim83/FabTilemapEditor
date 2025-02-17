using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

public class TextButton(float x, float y, float width, float height, string text, Action onClick)
{
    public Rectangle Rect { get; private set; } = new Rectangle(x, y, width, height);

    private bool isHovered;
    private bool isClicked;

    public void Update()
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        isHovered = Raylib.CheckCollisionPointRec(mousePos, Rect);
        isClicked = isHovered && Raylib.IsMouseButtonPressed(MouseButton.Left);

        if (isClicked)
            onClick?.Invoke();
    }

    public void Draw()
    {
        // Shadow effect
        Raylib.DrawRectangleRounded(new Rectangle(Rect.X + 4, Rect.Y + 4, Rect.Width, Rect.Height), 0.5f, 16, Constants.ShadowColor);

        // Button color based on state
        Color buttonColor = isHovered ? Constants.TitleBar : Constants.ButtonColor;

        Raylib.DrawRectangleRounded(Rect, 0.5f, 16, buttonColor);
        Raylib.DrawRectangleRoundedLinesEx(Rect, 0.5f, 16, 2, Color.Black);

        // Centered text
        int textWidth = Raylib.MeasureText(text, 16);
        int textX = (int)(Rect.X + (Rect.Width / 2) - (textWidth / 2));
        int textY = (int)(Rect.Y + (Rect.Height / 2) - 8);
        Raylib.DrawText(text, textX, textY, 16, Color.White);
    }
}

