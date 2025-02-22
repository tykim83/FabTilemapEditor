using FabTilemapEditor.Gui;
using FabTilemapEditor.Shared;
using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor.Layer;

public class LayerPanel(Rectangle rectangle, string name, int index, Action<LayerPanelState, int> onClick, Texture2D gearIcon, Texture2D eyeIcon, Texture2D visibleIcon)
{
    // Active
    private bool isActive = false;
    // GearMenu
    private bool showMenu = false;
    private Rectangle menuRect;
    private List<TextButton> menuButtons = [];
    private Rectangle gearIconRect = new Rectangle(rectangle.X + rectangle.Width - 32, rectangle.Y + 2, 28, 28);
    // Visibility
    private bool isVisible = true;
    private Rectangle visibleIconRect = new Rectangle(rectangle.X + 5, rectangle.Y + 2, 28, 28);

    public Rectangle Rect
    {
        get => rectangle;
        set
        {
            rectangle = value;
            gearIconRect = new Rectangle(rectangle.X + rectangle.Width - 32, rectangle.Y + 2, 28, 28);
            visibleIconRect = new Rectangle(rectangle.X + 5, rectangle.Y + 2, 28, 28);
        }
    }
    public int Index { get => index; set => index = value; }
    public string Name { get => name; }
    public bool IsVisible { get => isVisible; }
    public TextInputModal? InputModal { get; private set; } = null;

    public void GameStartup()
    {
        menuButtons.Clear();

        // Init Gear Menu
        menuRect = new Rectangle(rectangle.X + rectangle.Width + 5, rectangle.Y - 30, new Vector2(84, 80));
        menuButtons.Add(new TextButton(menuRect.X + 2, menuRect.Y + 2, 80, 25, "Clear", ClearLayer, false));
        menuButtons.Add(new TextButton(menuRect.X + 2, menuRect.Y + 27, 80, 25, "Rename", StartRemameLayer, false));
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

            // Toggle Visibility
            if (Raylib.CheckCollisionPointRec(mousePos, visibleIconRect))
            {
                isVisible = !isVisible;
                ToggleVisibility();
            }
        }

        // Update Text Input Modal
        InputModal?.Update();

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
        int textY = (int)(Rect.Y + Rect.Height / 2 - 8);
        Raylib.DrawText(name, textX, textY, 16, Color.White);

        // Draw Gear Icon
        Raylib.DrawTexture(gearIcon, (int)gearIconRect.X, (int)gearIconRect.Y, Color.DarkGray);

        // Draw Visible Icon
        if (isVisible)
            Raylib.DrawTexture(eyeIcon, (int)visibleIconRect.X, (int)visibleIconRect.Y, Color.DarkGray);
        else
            Raylib.DrawTexture(visibleIcon, (int)visibleIconRect.X, (int)visibleIconRect.Y, Color.DarkGray);

        // Draw Menu
        if (showMenu)
            DrawMenu();
    }

    public void ToggleActive() => isActive = !isActive;

    private void DrawMenu()
    {
        Raylib.DrawRectangleRec(menuRect, Color.DarkGray);

        foreach (var menuButton in menuButtons)
            menuButton.Draw();
    }

    private void StartRemameLayer()
    {
        showMenu = false;
        InputModal = new TextInputModal(name, RemameLayer);
    }

    // Layers Callbacks
    private void RemoveLayer()
    {
        showMenu = false;
        onClick.Invoke(LayerPanelState.Remove, index);
    }

    private void ClearLayer()
    {
        showMenu = false;
        onClick.Invoke(LayerPanelState.Clear, index);
    }

    private void ToggleVisibility()
    {
        onClick.Invoke(LayerPanelState.Visible, index);
    }

    private void RemameLayer(TextInputModalState state, string text)
    {
        InputModal = null;

        if (state is TextInputModalState.Close) return;

        name = text;
        onClick.Invoke(LayerPanelState.Rename, index);
    }
}
