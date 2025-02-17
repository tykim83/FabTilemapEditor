using FabTilemapEditor;
using Raylib_cs;


Raylib.InitWindow(Constants.ScreenWidth, Constants.ScreenHeight, "Hello World");
var tileset = new Tileset();
var layers = new Layers();
var tilemap = new Tilemap(tileset);

tileset.GameStartup();
layers.GameStartup();
tilemap.GameStartup();

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);

    tileset.HandleInput();
    layers.HandleInput();
    tilemap.HandleInput();

    tileset.GameRender();
    layers.GameRender();
    tilemap.GameRender();

    Raylib.EndDrawing();
}

Raylib.CloseWindow();
