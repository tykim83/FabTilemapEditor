using Raylib_cs;

namespace FabTilemapEditor.Shared;

public static class Constants
{
    public const int ScreenWidth = 1920;
    public const int ScreenHeight = 1080;

    public const int TileSize = 32;

    public static readonly Color BackgroundColor = new(70, 70, 70, 255);
    public static readonly Color PanelColor = new(51, 51, 51, 255);
    public static readonly Color ShadowColor = new(0, 0, 0, 80);
    public static readonly Color ButtonColor = new(85, 85, 85, 255);
    public static readonly Color TitleBar = Color.SkyBlue;
}
