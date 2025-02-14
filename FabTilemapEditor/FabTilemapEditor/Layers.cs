using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

public class Layers
{
    const int PANEL_X = 0;
    const int PANEL_Y = 700;
    const int PANEL_WIDTH = 600;
    const int PANEL_HEIGHT = 380;

    private Camera2D camera;
    private TextButton button;

    private List<string> layers = [];

    public Layers()
    {
        var availableSpace = Utilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Layers");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;
        var height = (int)availableSpace.Height;

        button = new TextButton(startingX + 10, startingY + height - 50, 150, 40, "Add Layer", () => { Console.WriteLine("click"); });
    }

    public void GameStartup()
    {
        layers.Add("background");

        float centerX = PANEL_X + PANEL_WIDTH / 2;
        float centerY = PANEL_Y + PANEL_HEIGHT / 2;

        camera = new Camera2D
        {
            Target = new Vector2(centerX, centerY),
            Offset = new Vector2(PANEL_X + PANEL_WIDTH / 2, PANEL_Y + PANEL_HEIGHT / 2),
            Rotation = 0.0f,
            Zoom = 1.0f,
        };
    }

    public void HandleInput()
    {
        button.Update();
        // Try Select Tile on Click
        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            //(var isInside, var worldMousePos) = IsMouseInsideTileset();
            //if (isInside && tileset.SelectedTile is not null)
            //{
            //    var tileX = (int)(worldMousePos.X - PANEL_MARGIN - TILEMAP_STARTING_X) / Constants.TILE_SIZE;
            //    var tileY = (int)(worldMousePos.Y - PANEL_MARGIN) / Constants.TILE_SIZE;

            //    tilemap[TilemapIndex(tileX, tileY)] = tileset.SelectedTile.Value;
            //}
        }
    }

    public void GameRender()
    {
        Raylib.BeginMode2D(camera);

        Utilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Layers");

        button.Draw();

        Raylib.EndMode2D();
    }

    //private (bool isInside, Vector2 worldMousePos) IsMouseInsideTileset()
    //{
    //    var mousePos = Raylib.GetMousePosition();
    //    var worldMousePos = Raylib.GetScreenToWorld2D(mousePos, camera);

    //    var isInside = worldMousePos.X >= (TILEMAP_STARTING_X + PANEL_MARGIN)
    //        && worldMousePos.Y >= PANEL_MARGIN
    //        && worldMousePos.X <= TILEMAP_STARTING_X + PANEL_MARGIN + (TILEMAP_WIDTH * Constants.TILE_SIZE)
    //        && worldMousePos.Y <= PANEL_MARGIN + (TILEMAP_HEIGHT * Constants.TILE_SIZE);

    //    return (isInside, worldMousePos);
    //}

    //private int TilemapIndex(int x, int y) => y * TILEMAP_WIDTH + x;

    //private void DrawTile(int tileID, int posX, int posY)
    //{
    //    int tilesPerRow = tileset.TilesetTexture.Width / Constants.TILE_SIZE;

    //    int tileX = tileID % tilesPerRow;
    //    int tileY = tileID / tilesPerRow;

    //    Rectangle source = new Rectangle(tileX * Constants.TILE_SIZE, tileY * Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
    //    Rectangle dest = new Rectangle(posX, posY, Constants.TILE_SIZE, Constants.TILE_SIZE);

    //    Raylib.DrawTexturePro(tileset.TilesetTexture, source, dest, new Vector2(0, 0), 0.0f, Color.White);
    //}
}
