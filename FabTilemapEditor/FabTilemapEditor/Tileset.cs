using FabTilemapEditor.Gui;
using FabTilemapEditor.Shared;
using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

public class Tileset
{
    const int PANEL_X = 0;
    const int PANEL_Y = 0;
    const int PANEL_WIDTH = 600;
    const int PANEL_HEIGHT = 700;

    private Texture2D tilesetTexture;
    private int? selectedTile;
    private Vector2? selectedTilePixelPos;
    private Camera2D camera;

    public int? SelectedTile { get => selectedTile; }
    public Texture2D TilesetTexture { get => tilesetTexture; }

    public void GameStartup()
    {
        // Calculate available space
        var availableSpace = GuiUtilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Tileset");

        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;
        var height = (int)availableSpace.Height;

        // Load tileset
        Image image = Raylib.LoadImage("./assets/Tileset_Grass.png");
        tilesetTexture = Raylib.LoadTextureFromImage(image);
        Raylib.UnloadImage(image);

        float tilesetWidth = tilesetTexture.Width;
        float tilesetHeight = tilesetTexture.Height;

        // Calculate zoom to fit width and height inside panel
        float zoomToFitWidth = width / tilesetWidth;
        float zoomToFitHeight = height / tilesetHeight;
        float finalZoom = Math.Min(zoomToFitWidth, zoomToFitHeight);

        // Camera for zooming/panning tileset
        camera = new Camera2D
        {
            Target = new Vector2(tilesetWidth / 2, tilesetHeight / 2),
            Offset = new Vector2(startingX + width / 2, startingY + height / 2),
            Rotation = 0.0f,
            Zoom = finalZoom
        };
    }

    public void HandleInput()
    {
        float zoomSpeed = 0.1f;
        float wheel = Raylib.GetMouseWheelMove();

        // Zoom in/out with mouse wheel
        if (wheel != 0)
        {
            camera.Zoom += wheel * zoomSpeed;
            camera.Zoom = Math.Clamp(camera.Zoom, 0.5f, 3.0f);
        }

        // Dragging with middle mouse button
        if (Raylib.IsMouseButtonDown(MouseButton.Middle))
        {
            Vector2 delta = Raylib.GetMouseDelta();
            camera.Target -= delta / camera.Zoom;
        }

        // Try Select Tile on Click
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            (var isInside, var worldMousePos) = IsMouseInsideTileset();
            if (isInside)
            {
                var tileX = (int)worldMousePos.X / Constants.TileSize;
                var tileY = (int)worldMousePos.Y / Constants.TileSize;

                var tilesPerRow = tilesetTexture.Width / Constants.TileSize;

                selectedTile = tileY * tilesPerRow + tileX;
                selectedTilePixelPos = new Vector2(tileX * Constants.TileSize, tileY * Constants.TileSize);
                Console.WriteLine($"Selected tile {selectedTile}");
            }
        }
    }

    public void GameRender()
    {
        var availableSpace = GuiUtilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Tileset");

        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;
        var height = (int)availableSpace.Height;

        Raylib.BeginScissorMode(startingX, startingY, width, height);

        Raylib.BeginMode2D(camera);

        Raylib.DrawTexture(tilesetTexture, 0, 0, Color.White);

        // Draw over highlight
        (var isInside, var worldMousePos) = IsMouseInsideTileset();
        if (isInside)
        {
            var tileX = (int)worldMousePos.X / Constants.TileSize;
            var tileY = (int)worldMousePos.Y / Constants.TileSize;

            Raylib.DrawRectangleLines(tileX * Constants.TileSize, tileY * Constants.TileSize, Constants.TileSize, Constants.TileSize, Color.Red);
        }

        // Draw Selected tile
        if (selectedTilePixelPos is not null)
            Raylib.DrawRectangleLines((int)selectedTilePixelPos.Value.X, (int)(selectedTilePixelPos.Value.Y), Constants.TileSize, Constants.TileSize, Color.Green);

        Raylib.EndMode2D();

        Raylib.EndScissorMode();
    }

    private (bool isInside, Vector2 worldMousePos) IsMouseInsideTileset()
    {
        var mousePos = Raylib.GetMousePosition();
        var worldMousePos = Raylib.GetScreenToWorld2D(mousePos, camera);

        var isInside = worldMousePos.X >= 0 && worldMousePos.Y >= 0 && worldMousePos.X <= tilesetTexture.Width && worldMousePos.Y <= tilesetTexture.Height;

        return (isInside, worldMousePos);
    }
}
