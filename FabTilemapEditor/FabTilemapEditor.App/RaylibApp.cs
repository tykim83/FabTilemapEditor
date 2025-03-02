using FabTilemapEditor.App.Layer;
using FabTilemapEditor.App.Shared;
using FabTilemapEditor.App.Tilemap;
using FabTilemapEditor.App.Tileset;
using Raylib_cs;

namespace FabTilemapEditor.App;

public class RaylibApp
{
    private static IFileService? _fileService;

    private static Tilesets? _tilesets;
    private static Layers? _Layers;
    private static Tilemaps? _Tilemaps;

    public static void Init(IFileService fileService)
    {
        _fileService = fileService;

        Raylib.InitWindow(Constants.ScreenWidth, Constants.ScreenHeight, "Hello World");
        _tilesets = new Tilesets(_fileService);
        _Layers = new Layers();
        _Tilemaps = new Tilemaps(_tilesets, _Layers, _fileService);

        _tilesets.GameStartup();
        _Layers.GameStartup();
        _Tilemaps.GameStartup();
    }

    public static void UpdateFrame()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);

        _Tilemaps?.Update();
        _tilesets?.Update();
        _Layers?.Update();

        _Tilemaps?.GameRender();
        _tilesets?.GameRender();
        _Layers?.GameRender();

        // Draw Tilemap Modal
        _Tilemaps?.InputModal?.Draw();
        if (_Layers is not null)
            foreach (var modal in _Layers.InputModals)
                modal.Draw();

        Raylib.EndDrawing();
    }
}
