using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

public static class Utilities
{
    private static int TILE_SIZE = 32;

    public static void DrawTile(Texture2D tileset, int pos_x, int pos_y, int texture_index_x, int texture_index_y)
    {
        var source = new Rectangle(texture_index_x * TILE_SIZE, texture_index_y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
        var dest = new Rectangle(pos_x, pos_y, TILE_SIZE, TILE_SIZE);
        var origin = new Vector2(0, 0);
        Raylib.DrawTexturePro(tileset, source, dest, origin, 0.0f, Color.White);
    }
}
