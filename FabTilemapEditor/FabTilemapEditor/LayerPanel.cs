using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

public class LayerPanel(Rectangle rectangle, string name, int index, Action<LayerPanelActionEnum, int> onClick, Texture2D gearIcon)
{
    public Rectangle Rect
    {
        get => rectangle;
        set
        {
            rectangle = value;
            gearIconRect = new Rectangle(rectangle.X + rectangle.Width - 32, rectangle.Y + 2, 28, 28);
        }
    }
    public int Index { get => index; set => index = value; }
    public string Name { get => name; }


    private bool isActive = false;
    private bool showMenu = false;
    private Rectangle menuRect;
    private List<TextButton> menuButtons = [];
    private Rectangle gearIconRect = new Rectangle(rectangle.X + rectangle.Width - 32, rectangle.Y + 2, 28, 28);

    public void ToggleActive() => isActive = !isActive;

    public void GameStartup()
    {
        menuButtons.Clear();

        // Init Gear Menu
        menuRect = new Rectangle(rectangle.X + rectangle.Width + 5, rectangle.Y - 30, new Vector2(84, 80));
        menuButtons.Add(new TextButton(menuRect.X + 2, menuRect.Y + 2, 80, 25, "Clear", ClearLayer, false));
        menuButtons.Add(new TextButton(menuRect.X + 2, menuRect.Y + 27, 80, 25, "Rename", () => { }, false));
        menuButtons.Add(new TextButton(menuRect.X + 2, menuRect.Y + 54, 80, 25, "Remove", RemoveLayer, false));
    }

    public void Update()
    {
        Vector2 mousePos = Raylib.GetMousePosition();

        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            // Close Gear Menu
            if (showMenu && !Raylib.CheckCollisionPointRec(mousePos, menuRect))
                showMenu = false;

            // Open Gear Menu
            if (Raylib.CheckCollisionPointRec(mousePos, gearIconRect))
                showMenu = !showMenu;
        }

        // Update Gear Menu
        if (showMenu)
            foreach (var menuButton in menuButtons)
                menuButton.Update();
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
        Raylib.DrawTexture(gearIcon, (int)gearIconRect.X, (int)gearIconRect.Y, Color.DarkGray);

        // Draw Menu
        if (showMenu)
            DrawMenu();
    }

    private void DrawMenu()
    {
        Raylib.DrawRectangleRec(menuRect, Color.DarkGray);

        foreach (var menuButton in menuButtons)
            menuButton.Draw();
    }

    private void RemoveLayer()
    {
        onClick.Invoke(LayerPanelActionEnum.Remove, index);
    }

    private void ClearLayer()
    {
        onClick.Invoke(LayerPanelActionEnum.Clear, index);
    }
}

public enum LayerPanelActionEnum
{
    Clear = 0,
    Remove,
}

