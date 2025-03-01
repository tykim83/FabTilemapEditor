using FabTilemapEditor.App.Shared;
using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor.App.Gui;

public class SelectBox
{
    public Rectangle Rect { get; private set; }

    private Rectangle gearIconRect;
    private Texture2D gearIcon;
    private bool isHovered;
    private readonly float roundnessValue;
    private readonly Action? onClick;
    private readonly List<string> options;
    private readonly int selected;

    public SelectBox(float x, float y, float width, float height, List<string> options, Action onClick, bool isRounded = true, int selected = 0)
    {
        this.options = options;
        this.selected = selected;
        Rect = new Rectangle(x, y, width, height);
        roundnessValue = isRounded ? 0.5f : 0;
        this.onClick = onClick;

        // Load Gear Icons
        gearIconRect = new Rectangle(Rect.X + Rect.Width - 32, Rect.Y + 2, 28, 28);
        Image image = Raylib.LoadImage("Assets/gear_icon.png");
        Raylib.ImageResize(ref image, 28, 28);
        gearIcon = Raylib.LoadTextureFromImage(image);
        Raylib.UnloadImage(image);
    }

    public void Update()
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        isHovered = Raylib.CheckCollisionPointRec(mousePos, Rect);

        if (isHovered && Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            onClick?.Invoke();
        }
    }

    public void Draw()
    {
        // Shadow effect
        Raylib.DrawRectangleRounded(new Rectangle(Rect.X + 4, Rect.Y + 4, Rect.Width, Rect.Height), roundnessValue, 16, Constants.ShadowColor);

        // Draw Select Box
        Raylib.DrawRectangleRounded(Rect, roundnessValue, 16, Constants.ButtonColor);
        Raylib.DrawRectangleRoundedLinesEx(Rect, roundnessValue, 16, 2, Color.Black);

        //  Draw Select Box Text
        int textX = (int)(Rect.X + 10);
        int textY = (int)(Rect.Y + Rect.Height / 2 - 8);
        Raylib.DrawText(options[selected], textX, textY, 16, Color.White);

        // Draw Gear Icon
        Raylib.DrawTexture(gearIcon, (int)gearIconRect.X, (int)gearIconRect.Y, Color.DarkGray);
    }
}
