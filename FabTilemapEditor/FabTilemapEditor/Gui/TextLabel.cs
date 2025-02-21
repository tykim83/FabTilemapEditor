using FabTilemapEditor.Shared;
using Raylib_cs;

namespace FabTilemapEditor.Gui;

public class TextLabel(float x, float y, float width, float height, string text, bool isRounded = true, bool hasBoder = true)
{
    public Rectangle Rect { get; private set; } = new Rectangle(x, y, width, height);

    private readonly float roundnessValue = isRounded ? 0.5f : 0;

    public void Draw()
    {
        // Shadow effect
        Raylib.DrawRectangleRounded(new Rectangle(Rect.X + 4, Rect.Y + 4, Rect.Width, Rect.Height), roundnessValue, 16, Constants.ShadowColor);

        Raylib.DrawRectangleRounded(Rect, roundnessValue, 16, Constants.ButtonColor);

        if (hasBoder)
            Raylib.DrawRectangleRoundedLinesEx(Rect, roundnessValue, 16, 2, Color.Black);

        // Centered text
        int textWidth = Raylib.MeasureText(text, 16);
        int textX = (int)(Rect.X + Rect.Width / 2 - textWidth / 2);
        int textY = (int)(Rect.Y + Rect.Height / 2 - 8);
        Raylib.DrawText(text, textX, textY, 16, Color.White);
    }
}

