using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

public class Tileset
{
    const int TILESET_PANEL_WIDTH = 600;
    const int TILESET_PANEL_HEIGHT = 600;
    const int PANEL_MARGIN = 25;

    private Texture2D tilesetTexture;
    private int? selectedTile;
    private Vector2? selectedTilePixelPos;
    private Camera2D camera;

    public int? SelectedTile { get => selectedTile; }
    public Texture2D TilesetTexture { get => tilesetTexture; }

    public void GameStartup()
    {
        // Load tileset
        Image image = Raylib.LoadImage("./assets/Tileset_Grass.png");
        tilesetTexture = Raylib.LoadTextureFromImage(image);
        Raylib.UnloadImage(image);

        float tilesetWidth = tilesetTexture.Width;
        float tilesetHeight = tilesetTexture.Height;

        // Calculate zoom to fit width and height inside panel
        float zoomToFitWidth = (TILESET_PANEL_WIDTH - PANEL_MARGIN * 2) / tilesetWidth;
        float zoomToFitHeight = (TILESET_PANEL_HEIGHT - PANEL_MARGIN * 2) / tilesetHeight;
        float finalZoom = Math.Min(zoomToFitWidth, zoomToFitHeight);

        // Camera for zooming/panning tileset
        camera = new Camera2D
        {
            Target = new Vector2(tilesetWidth / 2, tilesetHeight / 2),
            Offset = new Vector2(TILESET_PANEL_WIDTH / 2, TILESET_PANEL_HEIGHT / 2),
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
        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            (var isInside, var worldMousePos) = IsMouseInsideTileset();
            if (isInside)
            {
                var tileX = (int)worldMousePos.X / Constants.TILE_SIZE;
                var tileY = (int)worldMousePos.Y / Constants.TILE_SIZE;

                var tilesPerRow = tilesetTexture.Width / Constants.TILE_SIZE;

                selectedTile = tileY * tilesPerRow + tileX;
                selectedTilePixelPos = new Vector2(tileX * Constants.TILE_SIZE, tileY * Constants.TILE_SIZE);
                Console.WriteLine($"Selected tile {selectedTile}");
            }
        }
    }

    public void GameRender()
    {
        Raylib.DrawRectangle(0, 0, TILESET_PANEL_WIDTH, TILESET_PANEL_HEIGHT, Color.LightGray);

        Raylib.BeginScissorMode(PANEL_MARGIN, PANEL_MARGIN, TILESET_PANEL_WIDTH - (PANEL_MARGIN * 2), TILESET_PANEL_HEIGHT - (PANEL_MARGIN * 2));

        Raylib.BeginMode2D(camera);

        Raylib.DrawTexture(tilesetTexture, 0, 0, Color.White);

        // Draw over highlight
        (var isInside, var worldMousePos) = IsMouseInsideTileset();
        if (isInside)
        {
            var tileX = (int)worldMousePos.X / Constants.TILE_SIZE;
            var tileY = (int)worldMousePos.Y / Constants.TILE_SIZE;

            Raylib.DrawRectangleLines(tileX * Constants.TILE_SIZE, tileY * Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE, Color.Red);
        }

        // Draw Selected tile
        if (selectedTilePixelPos is not null)
            Raylib.DrawRectangleLines((int)selectedTilePixelPos.Value.X, (int)(selectedTilePixelPos.Value.Y), Constants.TILE_SIZE, Constants.TILE_SIZE, Color.Green);

        Raylib.EndMode2D();

        Raylib.EndScissorMode();

        Raylib.DrawRectangleLines(0, 0, TILESET_PANEL_WIDTH, TILESET_PANEL_HEIGHT, Color.Black);
    }

    private (bool isInside, Vector2 worldMousePos) IsMouseInsideTileset()
    {
        var mousePos = Raylib.GetMousePosition();
        var worldMousePos = Raylib.GetScreenToWorld2D(mousePos, camera);

        var isInside = worldMousePos.X >= 0 && worldMousePos.Y >= 0 && worldMousePos.X <= tilesetTexture.Width && worldMousePos.Y <= tilesetTexture.Height;

        return (isInside, worldMousePos);
    }
}
