using FabTilemapEditor;
using Raylib_cs;


Raylib.InitWindow(Constants.screenWidth, Constants.screenHeight, "Hello World");
var tileset = new Tileset();
var tilemap = new Tilemap();

tileset.GameStartup();
tilemap.GameStartup();

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);

    tileset.HandleInput();
    tilemap.HandleInput();

    tileset.GameRender();
    tilemap.GameRender();

    Raylib.EndDrawing();
}

Raylib.CloseWindow();
