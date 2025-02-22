namespace FabTilemapEditor.Tilemap;

public class TilemapLayer
{
    public string Name { get; set; } = string.Empty;
    public string Tileset { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
    public int[] Data { get; set; } = [];
}
