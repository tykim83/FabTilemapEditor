using FabTilemapEditor.App.Gui;
using FabTilemapEditor.App.Shared;
using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor.App.Tileset;

public class Tilesets(IFileService FileService)
{
    const int PANEL_X = 0;
    const int PANEL_Y = 0;
    const int PANEL_WIDTH = 600;
    const int PANEL_HEIGHT = 700;



    private int? selectedTile;
    private Vector2? selectedTilePixelPos;

    private Camera2D camera;

    private TextButton? addTilesetButton;

    public Dictionary<string, Texture2D> TilesetTexture { get; private set; } = [];
    public string SelectedTileset { get; private set; } = "Tileset_Grass.png";
    public int? SelectedTile { get => selectedTile; }

    public void GameStartup()
    {
        // Calculate available space
        var availableSpace = GuiUtilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Tileset");

        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;
        var height = (int)availableSpace.Height;

        // Init Button 
        addTilesetButton = new TextButton(startingX + 10, startingY + height - 40, 130, 30, "Add TileSet", AddTileSet);
        height -= 80;

        // Load tileset
        Image image = Raylib.LoadImage("./assets/Tileset_Grass.png");
        TilesetTexture.Add(SelectedTileset, Raylib.LoadTextureFromImage(image));
        Raylib.UnloadImage(image);

        float tilesetWidth = TilesetTexture[SelectedTileset].Width;
        float tilesetHeight = TilesetTexture[SelectedTileset].Height;

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

    public void Update()
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        if (!Raylib.CheckCollisionPointRec(mousePos, new Rectangle(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT)))
            return;

        float zoomSpeed = 0.1f;
        float wheel = Raylib.GetMouseWheelMove();

        // Update Button
        addTilesetButton?.Update();

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

                var tilesPerRow = TilesetTexture[SelectedTileset].Width / Constants.TileSize;

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
        var height = (int)availableSpace.Height - 50;

        // Draw Button
        addTilesetButton?.Draw();

        Raylib.BeginScissorMode(startingX, startingY, width, height);

        Raylib.BeginMode2D(camera);

        Raylib.DrawTexture(TilesetTexture[SelectedTileset], 0, 0, Color.White);

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
            Raylib.DrawRectangleLines((int)selectedTilePixelPos.Value.X, (int)selectedTilePixelPos.Value.Y, Constants.TileSize, Constants.TileSize, Color.Green);

        Raylib.EndMode2D();

        Raylib.EndScissorMode();
    }

    private (bool isInside, Vector2 worldMousePos) IsMouseInsideTileset()
    {
        var mousePos = Raylib.GetMousePosition();
        var worldMousePos = Raylib.GetScreenToWorld2D(mousePos, camera);

        var isInside = worldMousePos.X >= 0 && worldMousePos.Y >= 0 && worldMousePos.X <= TilesetTexture[SelectedTileset].Width && worldMousePos.Y <= TilesetTexture[SelectedTileset].Height;

        return (isInside, worldMousePos);
    }

    private async Task AddTileSet()
    {
        var filePath = await FileService.PickFileAsync();
        Image img = Raylib.LoadImage(filePath);
        SelectedTileset = Path.GetFileName(filePath);
        Console.WriteLine(SelectedTileset);

        TilesetTexture.Add(SelectedTileset, Raylib.LoadTextureFromImage(img));
    }
}
