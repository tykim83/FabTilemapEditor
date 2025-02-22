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
    private List<TilemapLayer> tilemapLayers = [];

    private TilemapMenu? menu;

    public TextInputModal? InputModal { get => menu?.InputModal; }

    public void GameStartup()
    {
        // Init tilemaps
        InitTilemapLayers();

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
        layers.SetupNotifyLayerSwapCallback(NotifyLayersSwap);

        layers.SetupRenameLayerCallback(UpdateTilemapLayersMetadata);
        layers.SetupToggleLayerVisibilityCallback(UpdateTilemapLayersMetadata);
    }

    public void HandleInput()
    {
        menu?.HandleInput();

        // Add Tile to Tilemap
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

                var index = layers.ActiveLayer;
                tilemapLayers[index].Data[TilemapIndex(tileX, tileY)] = tileset.SelectedTile.Value;
            }
        }
    }

    public void GameRender()
    {
        var availableSpace = GuiUtilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Tilemap");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;

        menu?.GameRender();

        Raylib.BeginMode2D(camera);

        Color gridColor = Color.Gray;

        // Draw Tilemap Layers
        DrawTilemapLayers(startingX, startingY);

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

    private void DrawTilemapLayers(int startingX, int startingY)
    {
        foreach (var layer in tilemapLayers)
        {
            if (!layer.IsVisible) continue;

            for (int i = 0; i < layer.Data.Length; i++)
            {
                int tileID = layer.Data[i];
                if (tileID != -1)
                {
                    int tileX = i % tilemapWidth;
                    int tileY = i / tilemapWidth;

                    int drawX = startingX + tileX * Constants.TileSize;
                    int drawY = startingY + tileY * Constants.TileSize;

                    DrawTile(tileID, drawX, drawY);
                }
            }
        }
    }

    private void InitTilemapLayers()
    {
        foreach (var layer in layers.LayerPanels)
        {
            var tilemapLayer = new TilemapLayer()
            {
                Data = new int[tilemapWidth * tilemapHeight],
                IsVisible = true,
                Name = layer.Name,
            };

            Array.Fill(tilemapLayer.Data, -1);

            tilemapLayers.Add(tilemapLayer);
        };
    }

    private void UpdateTilemapLayersMetadata(int index)
    {
        var tilemapLayer = tilemapLayers[index];
        var layer = layers.LayerPanels[index];

        tilemapLayer.IsVisible = layer.IsVisible;
        tilemapLayer.Name = layer.Name;
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
    private void AddLayer(int index)
    {
        var tilemapLayer = new TilemapLayer()
        {
            Data = new int[tilemapWidth * tilemapHeight],
            IsVisible = true,
            Name = layers.LayerPanels[index].Name,
        };

        Array.Fill(tilemapLayer.Data, -1);

        tilemapLayers.Add(tilemapLayer);
    }

    private void ClearLayer(int index)
    {
        Array.Fill(tilemapLayers[index].Data, -1);
    }

    private void RemoveLayer(int index)
    {
        tilemapLayers.RemoveAt(index);
    }

    //TODO: Implement Notify Layer Swap
    private void NotifyLayersSwap()
    {
        var tempTilemapLayers = new List<TilemapLayer>();

        foreach (var layer in layers.LayerPanels)
        {
            var tilemapLayer = tempTilemapLayers.First(x => x.Name == layer.Name);
            tempTilemapLayers.Add(tilemapLayer);
        };

        tilemapLayers = tempTilemapLayers;
    }
}

public class TilemapLayer
{
    public string Name { get; set; } = string.Empty;
    public string Tileset { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
    public int[] Data { get; set; } = [];
}