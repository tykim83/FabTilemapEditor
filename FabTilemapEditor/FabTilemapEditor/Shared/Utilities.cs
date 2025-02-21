using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor.Shared;

public static class Utilities
{
    public static void DrawTile(Texture2D tileset, int pos_x, int pos_y, int texture_index_x, int texture_index_y)
    {
        var source = new Rectangle(texture_index_x * Constants.TileSize, texture_index_y * Constants.TileSize, Constants.TileSize, Constants.TileSize);
        var dest = new Rectangle(pos_x, pos_y, Constants.TileSize, Constants.TileSize);
        var origin = new Vector2(0, 0);
        Raylib.DrawTexturePro(tileset, source, dest, origin, 0.0f, Color.White);
    }
}
