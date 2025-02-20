using FabTilemapEditor;
using Raylib_cs;


Raylib.InitWindow(Constants.ScreenWidth, Constants.ScreenHeight, "Hello World");
var tileset = new Tileset();
var layers = new Layers();
var tilemap = new Tilemap(tileset, layers);

tileset.GameStartup();
layers.GameStartup();
tilemap.GameStartup();

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);

    tilemap.HandleInput();
    tileset.HandleInput();
    layers.HandleInput();

    tilemap.GameRender();
    tileset.GameRender();
    layers.GameRender();

    Raylib.EndDrawing();
}

Raylib.CloseWindow();
