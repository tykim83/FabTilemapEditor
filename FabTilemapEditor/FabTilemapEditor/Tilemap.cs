using FabTilemapEditor.Gui;
using FabTilemapEditor.Shared;
using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

public class Tilemap(Tileset tileset, Layers layers)
{
    private const int PANEL_X = 600;
    private const int PANEL_Y = 0;
    private const int PANEL_WIDTH = 1320;
    private const int PANEL_HEIGHT = 1080;

    private int tilemapWidth = 16;
    private int tilemapHeight = 10;

    private Camera2D camera;
    private int[] tilemap;

    private TilemapMenu? menu;

    public TextInputModal? InputModal { get => menu?.InputModal; }

    public void GameStartup()
    {
        tilemap = new int[tilemapWidth * tilemapHeight];
        Array.Fill(tilemap, -1);

        // Calculate available space
        var availableSpace = GuiUtilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Tilemap");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;
        var height = (int)availableSpace.Height;

        // Init TilemaMenu
        menu = new TilemapMenu(startingX, startingY, tilemapWidth, tilemapHeight, UpdateTiles);
        menu.GameStartup();
        height -= 50;

        float zoomToFitWidth = width / (float)(tilemapWidth * Constants.TileSize);
        float zoomToFitHeight = height / (float)(tilemapHeight * Constants.TileSize);
        float finalZoom = Math.Min(zoomToFitWidth, zoomToFitHeight);
        float centerX = startingX + (tilemapWidth * Constants.TileSize) / 2;
        float centerY = startingY + (tilemapHeight * Constants.TileSize) / 2;

        camera = new Camera2D
        {
            Target = new Vector2(centerX, centerY),
            Offset = new Vector2(startingX + width / 2, startingY + height / 2),
            Rotation = 0.0f,
            Zoom = finalZoom
        };

        layers.SetupAddLayerCallback(AddLayer);
        layers.SetupClearLayerCallback(ClearLayer);
        layers.SetupRemoveLayerCallback(RemoveLayer);
        layers.SetupRenameLayerCallback(RenameLayer);
        layers.SetupToggleLayerVisibilityCallback(ToggleLayerVisibility);
        layers.SetupNotifyLayerSwapCallback(NotifyLayersSwap);
    }

    public void HandleInput()
    {
        menu?.HandleInput();

        // Try Select Tile on Click
        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            var availableSpace = GuiUtilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Tilemap");
            var startingX = (int)availableSpace.X;
            var startingY = (int)availableSpace.Y;

            (var isInside, var worldMousePos) = IsMouseInsideTileset(availableSpace);
            if (isInside && tileset.SelectedTile is not null)
            {
                int tileX = (int)((worldMousePos.X - startingX) / Constants.TileSize);
                int tileY = (int)((worldMousePos.Y - startingY) / Constants.TileSize);

                tilemap[TilemapIndex(tileX, tileY)] = tileset.SelectedTile.Value;
            }
        }
    }

    public void GameRender()
    {
        var availableSpace = GuiUtilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Tilemap");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;
        var height = (int)availableSpace.Height;

        menu?.GameRender();

        Raylib.BeginMode2D(camera);

        Color gridColor = Color.Gray;

        // Draw Tilemap
        for (int i = 0; i < tilemap.Length; i++)
        {
            int tileID = tilemap[i];
            if (tileID != -1)
            {
                int tileX = i % tilemapWidth;
                int tileY = i / tilemapWidth;

                int drawX = startingX + tileX * Constants.TileSize;
                int drawY = startingY + tileY * Constants.TileSize;

                DrawTile(tileID, drawX, drawY);
            }
        }

        // Draw vertical lines
        for (int i = 0; i <= tilemapWidth; i++)
        {
            int xPos = startingX + (i * Constants.TileSize);
            Raylib.DrawLine(xPos, startingY, xPos, startingY + (tilemapHeight * Constants.TileSize), gridColor);
        }

        // Draw horizontal lines
        for (int j = 0; j <= tilemapHeight; j++)
        {
            int yPos = startingY + (j * Constants.TileSize);
            Raylib.DrawLine(startingX, yPos, startingX + (tilemapWidth * Constants.TileSize), yPos, gridColor);
        }

        Raylib.EndMode2D();
    }

    private (bool isInside, Vector2 worldMousePos) IsMouseInsideTileset(Rectangle availableSpace)
    {
        var mousePos = Raylib.GetMousePosition();
        var worldMousePos = Raylib.GetScreenToWorld2D(mousePos, camera);

        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;

        var isInside = worldMousePos.X >= startingX
            && worldMousePos.Y >= startingY
            && worldMousePos.X <= startingX + (tilemapWidth * Constants.TileSize)
            && worldMousePos.Y <= startingY + (tilemapHeight * Constants.TileSize);

        return (isInside, worldMousePos);
    }

    private int TilemapIndex(int x, int y) => y * tilemapWidth + x;

    private void DrawTile(int tileID, int posX, int posY)
    {
        int tilesPerRow = tileset.TilesetTexture.Width / Constants.TileSize;

        int tileX = tileID % tilesPerRow;
        int tileY = tileID / tilesPerRow;

        Rectangle source = new Rectangle(tileX * Constants.TileSize, tileY * Constants.TileSize, Constants.TileSize, Constants.TileSize);
        Rectangle dest = new Rectangle(posX, posY, Constants.TileSize, Constants.TileSize);

        Raylib.DrawTexturePro(tileset.TilesetTexture, source, dest, new Vector2(0, 0), 0.0f, Color.White);
    }

    // TilemapMenu Callback Handlers
    //TODO: Implement change tiles widht and height
    private void UpdateTiles(TilemapMenuState state, int value)
    {
        Console.WriteLine($"Edit Tilemap tiles width or height: {value}");
    }

    // Layers Callback Handlers
    //TODO: Implement Add Layer
    private void AddLayer(int index, string layerName)
    {
        Console.WriteLine($"Add Layer: {index} - {layerName}");
    }

    //TODO: Implement Clear Layer
    private void ClearLayer(int index)
    {
        Console.WriteLine($"Clear Layer: {index}");
    }

    //TODO: Implement Remove Layer
    private void RemoveLayer(int index)
    {
        Console.WriteLine($"Remove Layer: {index}");
    }

    //TODO: Implement Rename Layer
    private void RenameLayer(int index, string layerName)
    {
        Console.WriteLine($"Rename Layer: {index} - {layerName}");
    }

    //TODO: Implement Toggle Layer Visibility
    private void ToggleLayerVisibility(int index)
    {
        Console.WriteLine($"Toggle Layer Visibility: {index}");
    }

    //TODO: Implement Notify Layer Swap
    private void NotifyLayersSwap()
    {
        Console.WriteLine("Layers Swap");
    }
}
