﻿using FabTilemapEditor.Gui;
using FabTilemapEditor.Shared;
using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

public class LayerPanel(Rectangle rectangle, string name, int index, Action<LayerPanelActionEnum, int, string> onClick, Texture2D gearIcon, Texture2D eyeIcon, Texture2D visibleIcon)
{
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
    // TextInputModal
    private TextInputModal? inputModal = null;

    public void ToggleActive() => isActive = !isActive;

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
                ToggleVisibility();
                isVisible = !isVisible;
            }
        }

        // Update Text Input Modal
        inputModal?.Update();

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

        // Draw Text Input Modal
        inputModal?.Draw();
    }

    private void DrawMenu()
    {
        Raylib.DrawRectangleRec(menuRect, Color.DarkGray);

        foreach (var menuButton in menuButtons)
            menuButton.Draw();
    }

    private void StartRemameLayer()
    {
        showMenu = false;
        inputModal = new TextInputModal(name, RemameLayer);
    }

    // Layers Callbacks
    private void RemoveLayer()
    {
        showMenu = false;
        onClick.Invoke(LayerPanelActionEnum.Remove, index, name);
    }

    private void ClearLayer()
    {
        showMenu = false;
        onClick.Invoke(LayerPanelActionEnum.Clear, index, name);
    }

    private void ToggleVisibility()
    {
        onClick.Invoke(LayerPanelActionEnum.Visible, index, name);
    }

    private void RemameLayer(TextInputModalState state, string text)
    {
        inputModal = null;

        if (state is TextInputModalState.Close) return;

        name = text;
        onClick.Invoke(LayerPanelActionEnum.Rename, index, name);
    }
}
