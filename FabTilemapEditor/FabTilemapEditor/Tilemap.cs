using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

public class Tilemap(Tileset tileset)
{
    const int PANEL_X = 600;
    const int PANEL_Y = 0;
    const int PANEL_WIDTH = 1320;
    const int PANEL_HEIGHT = 1080;

    const int TILEMAP_WIDTH = 15;
    const int TILEMAP_HEIGHT = 10;

    private Camera2D camera;
    private int[] tilemap = new int[TILEMAP_WIDTH * TILEMAP_HEIGHT];

    public void GameStartup()
    {
        Array.Fill(tilemap, -1);

        // Calculate available space
        var availableSpace = Utilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Tilemap");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;
        var height = (int)availableSpace.Height;

        float zoomToFitWidth = width / (TILEMAP_WIDTH * Constants.TileSize);
        float zoomToFitHeight = height / (TILEMAP_HEIGHT * Constants.TileSize);
        float finalZoom = Math.Min(zoomToFitWidth, zoomToFitHeight);
        float centerX = startingX + (TILEMAP_WIDTH * Constants.TileSize) / 2;
        float centerY = startingY + (TILEMAP_HEIGHT * Constants.TileSize) / 2;

        camera = new Camera2D
        {
            Target = new Vector2(centerX, centerY),
            Offset = new Vector2(startingX + width / 2, startingY + height / 2),
            Rotation = 0.0f,
            Zoom = finalZoom
        };
    }

    public void HandleInput()
    {
        // Try Select Tile on Click
        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            var availableSpace = Utilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Tilemap");
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
        var availableSpace = Utilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Tilemap");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;
        var height = (int)availableSpace.Height;

        Raylib.BeginMode2D(camera);

        Color gridColor = Color.Gray;

        // Draw Tilemap
        for (int i = 0; i < tilemap.Length; i++)
        {
            int tileID = tilemap[i];
            if (tileID != -1)
            {
                int tileX = i % TILEMAP_WIDTH;
                int tileY = i / TILEMAP_WIDTH;

                int drawX = startingX + tileX * Constants.TileSize;
                int drawY = startingY + tileY * Constants.TileSize;

                DrawTile(tileID, drawX, drawY);
            }
        }

        // Draw vertical lines
        for (int i = 0; i <= TILEMAP_WIDTH; i++)
        {
            int xPos = startingX + (i * Constants.TileSize);
            Raylib.DrawLine(xPos, startingY, xPos, startingY + (TILEMAP_HEIGHT * Constants.TileSize), gridColor);
        }

        // Draw horizontal lines
        for (int j = 0; j <= TILEMAP_HEIGHT; j++)
        {
            int yPos = startingY + (j * Constants.TileSize);
            Raylib.DrawLine(startingX, yPos, startingX + (TILEMAP_WIDTH * Constants.TileSize), yPos, gridColor);
        }

        Raylib.EndMode2D();

        // Draw a black border around the tilemap
        Raylib.DrawRectangleLines(startingX, startingY, width, height, Color.Red);
    }

    private (bool isInside, Vector2 worldMousePos) IsMouseInsideTileset(Rectangle availableSpace)
    {
        var mousePos = Raylib.GetMousePosition();
        var worldMousePos = Raylib.GetScreenToWorld2D(mousePos, camera);

        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;

        var isInside = worldMousePos.X >= startingX
            && worldMousePos.Y >= startingY
            && worldMousePos.X <= startingX + (TILEMAP_WIDTH * Constants.TileSize)
            && worldMousePos.Y <= startingY + (TILEMAP_HEIGHT * Constants.TileSize);

        return (isInside, worldMousePos);
    }

    private int TilemapIndex(int x, int y) => y * TILEMAP_WIDTH + x;

    private void DrawTile(int tileID, int posX, int posY)
    {
        int tilesPerRow = tileset.TilesetTexture.Width / Constants.TileSize;

        int tileX = tileID % tilesPerRow;
        int tileY = tileID / tilesPerRow;

        Rectangle source = new Rectangle(tileX * Constants.TileSize, tileY * Constants.TileSize, Constants.TileSize, Constants.TileSize);
        Rectangle dest = new Rectangle(posX, posY, Constants.TileSize, Constants.TileSize);

        Raylib.DrawTexturePro(tileset.TilesetTexture, source, dest, new Vector2(0, 0), 0.0f, Color.White);
    }
}
