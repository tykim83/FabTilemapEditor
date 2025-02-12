using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

public static class Utilities
{
    public static void DrawTile(Texture2D tileset, int pos_x, int pos_y, int texture_index_x, int texture_index_y)
    {
        var source = new Rectangle(texture_index_x * Constants.TILE_SIZE, texture_index_y * Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
        var dest = new Rectangle(pos_x, pos_y, Constants.TILE_SIZE, Constants.TILE_SIZE);
        var origin = new Vector2(0, 0);
        Raylib.DrawTexturePro(tileset, source, dest, origin, 0.0f, Color.White);
    }

    public static Rectangle RenderSectionUI(int startingX, int startingY, int panelWidth, int panelHeight, string titleText)
    {
        int panelX = startingX + 10;
        int panelY = startingY + 10;
        int availablePanelWidth = panelWidth - 20;
        int availablePanelHeight = panelHeight - 20;

        var backgroundColor = new Color(70, 70, 70, 255);
        var panelColor = new Color(51, 51, 51, 255);
        var shadowColor = new Color(0, 0, 0, 80);

        // Draw Background
        Raylib.DrawRectangle(startingX, startingY, panelWidth, panelHeight, backgroundColor);

        // Draw main panel
        Raylib.DrawRectangleRoundedLinesEx(new Rectangle(panelX + 6, panelY + 6, availablePanelWidth - 12, availablePanelHeight - 12), 0.05f, 32, 6, panelColor);

        // Draw main panel background
        Raylib.DrawRectangleRounded(new Rectangle(panelX + 6, panelY + 46, availablePanelWidth - 12, availablePanelHeight - 52), 0.05f, 32, panelColor);

        // Draw the main panel shadow
        Raylib.DrawRectangleRoundedLinesEx(new Rectangle(panelX + 8, panelY + 12, availablePanelWidth - 16, availablePanelHeight - 20), 0.05f, 32, 4, shadowColor);

        // Draw title bar
        var titleBar = new Rectangle(panelX + 6, panelY + 6, availablePanelWidth - 12, 47);
        Raylib.DrawRectangleRounded(titleBar, 0.5f, 32, Color.SkyBlue);
        Raylib.DrawRectangle(panelX + 6, panelY + 46, availablePanelWidth - 12, 7, panelColor);

        // Complete Shadow
        Raylib.DrawLineEx(new Vector2(panelX + 7, panelY + 46), new Vector2(panelX + 7, panelY + 53), 2, shadowColor);
        Raylib.DrawLineEx(new Vector2(availablePanelWidth + 3, panelY + 46), new Vector2(availablePanelWidth + 3, panelY + 53), 2, shadowColor);

        // Title Text
        var textWidth = Raylib.MeasureText(titleText, 24);
        var textX = (int)(panelX + (availablePanelWidth / 2) - (textWidth / 2));
        var textY = panelY + 15;
        Raylib.DrawText(titleText, textX, textY, 24, Color.White);

        // Return Available space
        return new Rectangle(panelX + 18, panelY + 58, availablePanelWidth - 36, availablePanelHeight - 76);
    }
}
