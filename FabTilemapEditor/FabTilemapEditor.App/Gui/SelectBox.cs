using FabTilemapEditor.App.Shared;
using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor.App.Gui;

public class SelectBox
{
    public Rectangle Rect { get; private set; }
    public bool IsOpen { get; private set; }

    private List<Rectangle> optionsRect = [];
    private Rectangle gearIconRect;
    private Texture2D gearIcon;
    private readonly float roundnessValue;
    private readonly Action<string>? onClick;
    private readonly List<string> options;
    private int selected;

    public SelectBox(float x, float y, float width, float height, List<string> options, Action<string> onClick, bool isRounded = true, int selected = 0)
    {
        this.options = options;
        this.selected = selected;
        Rect = new Rectangle(x, y, width, height);
        roundnessValue = isRounded ? 0.5f : 0;
        this.onClick = onClick;

        // Load Icon
        gearIconRect = new Rectangle(Rect.X + Rect.Width - 32, Rect.Y + 2, 28, 28);
        Image image = Raylib.LoadImage("Assets/arrow-down-icon.png");
        Raylib.ImageResize(ref image, 28, 28);
        gearIcon = Raylib.LoadTextureFromImage(image);
        Raylib.UnloadImage(image);

        for (int i = 0; i < options.Count; i++)
        {
            optionsRect.Add(new Rectangle(x, y + 30 + (i * 40), width + 50, 40));
        }
    }

    public void AddtOption(string option)
    {
        this.options.Add(option);
        optionsRect.Add(new Rectangle(Rect.X, Rect.Y + 30 + (optionsRect.Count * 40), Rect.Width + 50, 40));
        selected = this.options.Count - 1;
    }

    public void Update()
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        var isInside = Raylib.CheckCollisionPointRec(mousePos, Rect);

        if (isInside && Raylib.IsMouseButtonReleased(MouseButton.Left))
            IsOpen = !IsOpen;

        if (!isInside && IsOpen && Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            for (int i = 0; i < optionsRect.Count; i++)
            {
                Rectangle optionRec = optionsRect[i];
                var isInsideOption = Raylib.CheckCollisionPointRec(mousePos, optionRec);

                if (isInsideOption && Raylib.IsMouseButtonReleased(MouseButton.Left))
                {
                    selected = i;
                    IsOpen = false;
                    onClick?.Invoke(options[selected]);
                    return;
                }
            }

            IsOpen = false;
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

        // Draw Icon
        Raylib.DrawTexture(gearIcon, (int)gearIconRect.X, (int)gearIconRect.Y, Color.DarkGray);

        // Options
        if (IsOpen)
        {
            for (int i = 0; i < optionsRect.Count; i++)
            {
                Rectangle optionRect = optionsRect[i];
                Color backgroudColor = selected == i ? Constants.TitleBar : Color.White;
                Raylib.DrawRectangleRec(optionRect, backgroudColor);

                // Text
                Color textColor = selected == i ? Color.White : Color.Black;
                int optionTextX = (int)(optionRect.X + 10);
                int optionTextY = (int)(optionRect.Y + optionRect.Height / 2 - 8);
                Raylib.DrawText(options[i], optionTextX, optionTextY, 16, textColor);
            }

            Raylib.DrawRectangleLinesEx(new Rectangle(Rect.X, Rect.Y + 30, Rect.Width + 50, optionsRect.Count * 40), 2, Color.DarkGray);
        }
    }
}
