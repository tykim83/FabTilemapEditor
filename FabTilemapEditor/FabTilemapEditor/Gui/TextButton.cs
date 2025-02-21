using FabTilemapEditor.Shared;
using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor.Gui;

public class TextButton(float x, float y, float width, float height, string text, Action onClick, bool isRounded = true)
{
    public Rectangle Rect { get; private set; } = new Rectangle(x, y, width, height);

    private bool isHovered;
    private readonly float roundnessValue = isRounded ? 0.5f : 0;

    private float clickCooldownTimer = 0f;
    private const float CLICK_COOLDOWN = 0.4f;

    public void Update()
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        isHovered = Raylib.CheckCollisionPointRec(mousePos, Rect);

        clickCooldownTimer += Raylib.GetFrameTime();

        if (isHovered && Raylib.IsMouseButtonReleased(MouseButton.Left) && clickCooldownTimer >= CLICK_COOLDOWN)
        {
            clickCooldownTimer = 0f;
            onClick?.Invoke();
        }
    }

    public void Draw()
    {
        // Shadow effect
        Raylib.DrawRectangleRounded(new Rectangle(Rect.X + 4, Rect.Y + 4, Rect.Width, Rect.Height), roundnessValue, 16, Constants.ShadowColor);

        // Button color based on state
        Color buttonColor = isHovered ? Constants.TitleBar : Constants.ButtonColor;

        Raylib.DrawRectangleRounded(Rect, roundnessValue, 16, buttonColor);
        Raylib.DrawRectangleRoundedLinesEx(Rect, roundnessValue, 16, 2, Color.Black);

        // Centered text
        int textWidth = Raylib.MeasureText(text, 16);
        int textX = (int)(Rect.X + Rect.Width / 2 - textWidth / 2);
        int textY = (int)(Rect.Y + Rect.Height / 2 - 8);
        Raylib.DrawText(text, textX, textY, 16, Color.White);
    }
}

