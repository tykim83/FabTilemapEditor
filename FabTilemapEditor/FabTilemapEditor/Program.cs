using FabTilemapEditor.Layer;
using FabTilemapEditor.Shared;
using FabTilemapEditor.Tilemap;
using FabTilemapEditor.Tileset;
using Raylib_cs;


Raylib.InitWindow(Constants.ScreenWidth, Constants.ScreenHeight, "Hello World");
var tilesets = new Tilesets();
var layers = new Layers();
var tilemap = new Tilemap(tilesets, layers);

tilesets.GameStartup();
layers.GameStartup();
tilemap.GameStartup();

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);

    tilemap.Update();
    tilesets.Update();
    layers.Update();

    tilemap.GameRender();
    tilesets.GameRender();
    layers.GameRender();

    // Draw Tilemap Modal
    tilemap.InputModal?.Draw();
    foreach (var modal in layers.InputModals)
        modal.Draw();

    Raylib.EndDrawing();
}

Raylib.CloseWindow();
