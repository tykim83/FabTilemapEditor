using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

public class Tilemap(Tileset tileset)
{
    const int TILEMAP_STARTING_WIDTH = 601;
    const int TILEMAP_PANEL_WIDTH = 1320;
    const int TILEMAP_PANEL_HEIGHT = 1080;
    const int PANEL_MARGIN = 100;

    const int TILEMAP_WIDTH = 15;
    const int TILEMAP_HEIGHT = 10;

    private Camera2D camera;
    private int[] tilemap = new int[TILEMAP_WIDTH * TILEMAP_HEIGHT];

    public void GameStartup()
    {
        Array.Fill(tilemap, -1);

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
            (var isInside, var worldMousePos) = IsMouseInsideTileset();
            if (isInside && tileset.SelectedTile is not null)
            {
                var tileX = (int)(worldMousePos.X - PANEL_MARGIN - TILEMAP_STARTING_WIDTH) / Constants.TILE_SIZE;
                var tileY = (int)(worldMousePos.Y - PANEL_MARGIN) / Constants.TILE_SIZE;

                tilemap[TilemapIndex(tileX, tileY)] = tileset.SelectedTile.Value;
            }
        }
    }

    public void GameRender()
    {
        Raylib.BeginMode2D(camera);

        Color gridColor = Color.Gray;

        // Draw Tilemap
        for (int i = 0; i < tilemap.Length; i++)
        {
            int tileID = tilemap[i];
            if (tileID != -1)
            {
                int tileX = i % TILEMAP_WIDTH;
                int tileY = i / TILEMAP_HEIGHT;

                int drawX = 601 + PANEL_MARGIN + tileX * Constants.TILE_SIZE;
                int drawY = PANEL_MARGIN + tileY * Constants.TILE_SIZE;

                DrawTile(tileID, drawX, drawY);
            }
        }

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

    private (bool isInside, Vector2 worldMousePos) IsMouseInsideTileset()
    {
        var mousePos = Raylib.GetMousePosition();
        var worldMousePos = Raylib.GetScreenToWorld2D(mousePos, camera);

        var isInside = worldMousePos.X >= (TILEMAP_STARTING_WIDTH + PANEL_MARGIN)
            && worldMousePos.Y >= PANEL_MARGIN
            && worldMousePos.X <= TILEMAP_STARTING_WIDTH + PANEL_MARGIN + (TILEMAP_WIDTH * Constants.TILE_SIZE)
            && worldMousePos.Y <= PANEL_MARGIN + (TILEMAP_HEIGHT * Constants.TILE_SIZE);

        return (isInside, worldMousePos);
    }

    private int TilemapIndex(int x, int y) => y * TILEMAP_WIDTH + x;

    private void DrawTile(int tileID, int posX, int posY)
    {
        int tilesPerRow = tileset.TilesetTexture.Width / Constants.TILE_SIZE;

        int tileX = tileID % tilesPerRow;
        int tileY = tileID / tilesPerRow;

        Rectangle source = new Rectangle(tileX * Constants.TILE_SIZE, tileY * Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
        Rectangle dest = new Rectangle(posX, posY, Constants.TILE_SIZE, Constants.TILE_SIZE);

        Console.WriteLine(source);
        Console.WriteLine(dest);

        Raylib.DrawTexturePro(tileset.TilesetTexture, source, dest, new Vector2(0, 0), 0.0f, Color.White);
    }
}
