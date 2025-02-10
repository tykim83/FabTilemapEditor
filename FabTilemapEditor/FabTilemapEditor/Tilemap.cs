using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

public class Tilemap
{
    const int TILEMAP_STARTING_WIDTH = 601;
    const int TILEMAP_PANEL_WIDTH = 1320;
    const int TILEMAP_PANEL_HEIGHT = 1080;
    const int PANEL_MARGIN = 100;

    const int TILEMAP_WIDTH = 15;
    const int TILEMAP_HEIGHT = 10;

    private Camera2D camera;

    public void GameStartup()
    {
        float availableWidth = TILEMAP_PANEL_WIDTH - (PANEL_MARGIN * 2);
        float availableHeight = TILEMAP_PANEL_HEIGHT - (PANEL_MARGIN * 2);

        float zoomToFitWidth = availableWidth / (TILEMAP_WIDTH * Constants.TILE_SIZE);
        float zoomToFitHeight = availableHeight / (TILEMAP_HEIGHT * Constants.TILE_SIZE);
        float finalZoom = Math.Min(zoomToFitWidth, zoomToFitHeight);
        float centerX = TILEMAP_STARTING_WIDTH + PANEL_MARGIN + (TILEMAP_WIDTH * Constants.TILE_SIZE) / 2;
        float centerY = PANEL_MARGIN + (TILEMAP_HEIGHT * Constants.TILE_SIZE) / 2;

        camera = new Camera2D
        {
            Target = new Vector2(centerX, centerY),
            Offset = new Vector2(TILEMAP_STARTING_WIDTH + TILEMAP_PANEL_WIDTH / 2, TILEMAP_PANEL_HEIGHT / 2),
            Rotation = 0.0f,
            Zoom = finalZoom
        };
    }



    public void HandleInput()
    {
        // Try Select Tile on Click
        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {

        }
    }

    public void GameRender()
    {
        Raylib.BeginMode2D(camera);

        Color gridColor = Color.Gray;

        // Draw vertical lines
        for (int i = 0; i <= TILEMAP_WIDTH; i++)
        {
            int xPos = TILEMAP_STARTING_WIDTH + PANEL_MARGIN + (i * Constants.TILE_SIZE);
            Raylib.DrawLine(xPos, PANEL_MARGIN, xPos, PANEL_MARGIN + (TILEMAP_HEIGHT * Constants.TILE_SIZE), gridColor);
        }

        // Draw horizontal lines
        for (int j = 0; j <= TILEMAP_HEIGHT; j++)
        {
            int yPos = PANEL_MARGIN + (j * Constants.TILE_SIZE);
            Raylib.DrawLine(TILEMAP_STARTING_WIDTH + PANEL_MARGIN, yPos, TILEMAP_STARTING_WIDTH + PANEL_MARGIN + (TILEMAP_WIDTH * Constants.TILE_SIZE), yPos, gridColor);
        }

        Raylib.EndMode2D();

        // Draw a black border around the tilemap
        Raylib.DrawRectangleLines(601, 0, TILEMAP_PANEL_WIDTH, TILEMAP_PANEL_HEIGHT, Color.Black);
    }
}
