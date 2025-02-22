using FabTilemapEditor.Gui;
using FabTilemapEditor.Layer;
using FabTilemapEditor.Shared;
using FabTilemapEditor.Tileset;
using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor.Tilemap;

public class Tilemap(Tilesets tilesets, Layers layers)
{
    private const int PANEL_X = 600;
    private const int PANEL_Y = 0;
    private const int PANEL_WIDTH = 1320;
    private const int PANEL_HEIGHT = 1080;

    private Camera2D camera;
    private List<TilemapLayer> tilemapLayers = [];
    private readonly Canvas canvas = new()
    {
        Width = 16 * Constants.TileSize,
        Height = 10 * Constants.TileSize,
        TilesWidth = 16,
        TilesHeight = 10
    };

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
        menu = new TilemapMenu(startingX, startingY, canvas.TilesWidth, canvas.TilesHeight, UpdateTiles);
        menu.GameStartup();

        // Update Camera
        UpdateCamera();

        // Setup Callbacks
        layers.SetupAddLayerCallback(AddLayer);
        layers.SetupClearLayerCallback(ClearLayer);
        layers.SetupRemoveLayerCallback(RemoveLayer);
        layers.SetupNotifyLayerSwapCallback(NotifyLayersSwap);
        layers.SetupRenameLayerCallback(UpdateTilemapLayersMetadata);
        layers.SetupToggleLayerVisibilityCallback(UpdateTilemapLayersMetadata);
    }

    public void Update()
    {
        menu?.Update();

        // Add Tile to Tilemap
        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            var availableSpace = GuiUtilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Tilemap");
            var startingX = (int)availableSpace.X;
            var startingY = (int)availableSpace.Y;

            (var isInside, var worldMousePos) = IsMouseInsideTileset(availableSpace);
            if (isInside && tilesets.SelectedTile is not null)
            {
                int tileX = (int)((worldMousePos.X - startingX) / Constants.TileSize);
                int tileY = (int)((worldMousePos.Y - startingY) / Constants.TileSize);

                var index = layers.ActiveLayer;
                tilemapLayers[index].Data[TilemapIndex(tileX, tileY)] = tilesets.SelectedTile.Value;
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

        // Draw Tilemap Layers
        DrawTilemapLayers(startingX, startingY);

        // Draw vertical lines
        for (int i = 0; i <= canvas.TilesWidth; i++)
        {
            int xPos = startingX + i * Constants.TileSize;
            Raylib.DrawLine(xPos, startingY, xPos, startingY + canvas.TilesHeight * Constants.TileSize, Color.Gray);
        }

        // Draw horizontal lines
        for (int j = 0; j <= canvas.TilesHeight; j++)
        {
            int yPos = startingY + j * Constants.TileSize;
            Raylib.DrawLine(startingX, yPos, startingX + canvas.TilesWidth * Constants.TileSize, yPos, Color.Gray);
        }

        Raylib.EndMode2D();
    }

    private void UpdateCamera()
    {
        var availableSpace = GuiUtilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Tilemap");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y + 70;
        var width = (int)availableSpace.Width;
        var height = (int)availableSpace.Height - 70;

        Raylib.DrawRectangleLines(startingX, startingY, width, height, Color.Red);

        float zoomToFitWidth = width / (float)(canvas.TilesWidth * Constants.TileSize);
        float zoomToFitHeight = height / (float)(canvas.TilesHeight * Constants.TileSize);
        float finalZoom = Math.Min(zoomToFitWidth, zoomToFitHeight);
        float centerX = startingX + canvas.TilesWidth * Constants.TileSize / 2;
        float centerY = startingY + canvas.TilesHeight * Constants.TileSize / 2;

        camera = new Camera2D
        {
            Target = new Vector2(centerX, centerY - 70),
            Offset = new Vector2(startingX + width / 2, startingY + height / 2),
            Rotation = 0.0f,
            Zoom = finalZoom
        };
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
                    int tileX = i % canvas.TilesWidth;
                    int tileY = i / canvas.TilesWidth;

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
                Data = new int[canvas.TilesWidth * canvas.TilesHeight],
                IsVisible = true,
                Name = layer.Name,
            };

            Array.Fill(tilemapLayer.Data, -1);

            tilemapLayers.Add(tilemapLayer);
        };
    }

    private void ResizeTilemap(int newWidth, int newHeight)
    {
        foreach (var tilemapLayer in tilemapLayers)
        {
            int[] newData = new int[newWidth * newHeight];
            Array.Fill(newData, -1);

            // Copy over existing tile data
            for (int y = 0; y < Math.Min(canvas.TilesHeight, newHeight); y++)
            {
                for (int x = 0; x < Math.Min(canvas.TilesWidth, newWidth); x++)
                {
                    int oldIndex = y * canvas.TilesWidth + x;
                    int newIndex = y * newWidth + x;
                    newData[newIndex] = tilemapLayer.Data[oldIndex];
                }
            }

            tilemapLayer.Data = newData;
        }
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
            && worldMousePos.X <= startingX + canvas.TilesWidth * Constants.TileSize
            && worldMousePos.Y <= startingY + canvas.TilesHeight * Constants.TileSize;

        return (isInside, worldMousePos);
    }

    private int TilemapIndex(int x, int y) => y * canvas.TilesWidth + x;

    private void DrawTile(int tileID, int posX, int posY)
    {
        int tilesPerRow = tilesets.TilesetTexture.Width / Constants.TileSize;

        int tileX = tileID % tilesPerRow;
        int tileY = tileID / tilesPerRow;

        Rectangle source = new Rectangle(tileX * Constants.TileSize, tileY * Constants.TileSize, Constants.TileSize, Constants.TileSize);
        Rectangle dest = new Rectangle(posX, posY, Constants.TileSize, Constants.TileSize);

        Raylib.DrawTexturePro(tilesets.TilesetTexture, source, dest, new Vector2(0, 0), 0.0f, Color.White);
    }

    // TilemapMenu Callback Handlers
    private void UpdateTiles(TilemapMenuState state, int value)
    {
        if (state is TilemapMenuState.EditTilesWidth)
        {
            ResizeTilemap(value, canvas.TilesHeight);
            canvas.TilesWidth = value;
        }
        else if (state is TilemapMenuState.EditTilesHeight)
        {
            ResizeTilemap(canvas.TilesWidth, value);
            canvas.TilesHeight = value;
        }
        UpdateCamera();
    }

    // Layers Callback Handlers
    private void AddLayer(int index)
    {
        var tilemapLayer = new TilemapLayer()
        {
            Data = new int[canvas.TilesWidth * canvas.TilesHeight],
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

    private void NotifyLayersSwap()
    {
        var tempTilemapLayers = new List<TilemapLayer>();

        foreach (var layer in layers.LayerPanels)
        {
            var tilemapLayer = tilemapLayers.First(x => x.Name == layer.Name);
            tempTilemapLayers.Add(tilemapLayer);
        };
        tilemapLayers.Clear();
        tilemapLayers = tempTilemapLayers;
    }
}
