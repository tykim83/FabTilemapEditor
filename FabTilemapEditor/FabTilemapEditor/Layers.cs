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

    // Layers fields
    private int activeLayer = 0;
    private List<string> layers = [];
    private List<Rectangle> layersRectangles = [];

    // Drag fields
    private int? draggingLayerIndex = null;
    private float dragOffsetY = 0;
    private float dragStartTime = 0f;
    private bool isDragging = false;
    private const float DRAG_DELAY = 0.15f;

    public Layers()
    {
        var availableSpace = Utilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Layers");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;
        var height = (int)availableSpace.Height;

        button = new TextButton(startingX + 10, startingY + height - 50, 130, 30, "Add Layer", AddLayer);
    }

    public void GameStartup()
    {
        layers.Add("background");
        layers.Add("test");

        float centerX = PANEL_X + PANEL_WIDTH / 2;
        float centerY = PANEL_Y + PANEL_HEIGHT / 2;

        camera = new Camera2D
        {
            Target = new Vector2(centerX, centerY),
            Offset = new Vector2(PANEL_X + PANEL_WIDTH / 2, PANEL_Y + PANEL_HEIGHT / 2),
            Rotation = 0.0f,
            Zoom = 1.0f,
        };

        UpdateLayerReacts();
    }

    public void HandleInput()
    {
        button.Update();

        Vector2 mousePos = Raylib.GetMousePosition();

        // Init Drag Layer with Delay
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            for (int i = 0; i < layersRectangles.Count; i++)
            {
                if (Raylib.CheckCollisionPointRec(mousePos, layersRectangles[i]))
                {
                    draggingLayerIndex = i;
                    dragOffsetY = mousePos.Y - layersRectangles[i].Y;
                    dragStartTime = (float)Raylib.GetTime();
                    isDragging = false;
                    break;
                }
            }
        }

        // Start Drag Layer
        if (draggingLayerIndex.HasValue && !isDragging)
        {
            if (Raylib.GetTime() - dragStartTime > DRAG_DELAY)
                isDragging = true;
        }

        // Drag Layer 
        if (isDragging && draggingLayerIndex.HasValue)
        {
            int index = draggingLayerIndex.Value;
            var draggedRect = layersRectangles[index];
            draggedRect.Y = mousePos.Y - dragOffsetY;
            layersRectangles[index] = draggedRect;

            // Swap layers
            for (int i = 0; i < layersRectangles.Count; i++)
            {
                if (i != index)
                {
                    float halfHeight = layersRectangles[i].Height / 2;
                    float centerY = layersRectangles[i].Y + halfHeight;

                    if ((index > i && layersRectangles[index].Y < centerY) ||
                        (index < i && layersRectangles[index].Y + layersRectangles[index].Height > centerY))
                    {
                        if (activeLayer == i)
                            activeLayer = index;
                        else if (activeLayer == index)
                            activeLayer = i;

                        (layers[index], layers[i]) = (layers[i], layers[index]);
                        UpdateLayerReacts();
                        draggingLayerIndex = i;
                        break;
                    }
                }
            }
        }

        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            // Set active if only click
            if (!isDragging && draggingLayerIndex.HasValue)
            {
                for (int i = 0; i < layersRectangles.Count; i++)
                {
                    if (Raylib.CheckCollisionPointRec(mousePos, layersRectangles[i]))
                    {
                        activeLayer = i;
                        break;
                    }
                }
            }

            // Release Drag Layer
            draggingLayerIndex = null;
            dragOffsetY = 0;
            isDragging = false;

            UpdateLayerReacts();
        }
    }

    public void GameRender()
    {
        Raylib.BeginMode2D(camera);

        Utilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Layers");

        for (var i = 0; i < layers.Count; i++)
        {
            var layerRectangle = layersRectangles[i];
            var text = layers[i];

            var color = i == activeLayer ? Color.LightGray : Color.DarkGray;
            Raylib.DrawRectangleRec(layerRectangle, color);
            Raylib.DrawRectangleLinesEx(layerRectangle, 1, Color.LightGray);

            // Centered text
            int textX = (int)layerRectangle.X + 40;
            int textY = (int)(layerRectangle.Y + (layerRectangle.Height / 2) - 8);
            Raylib.DrawText(text, textX, textY, 16, Color.White);
        }

        button.Draw();

        Raylib.EndMode2D();
    }

    private void UpdateLayerReacts()
    {
        layersRectangles.Clear();

        var availableSpace = Utilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Layers");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;

        for (var i = 0; i < layers.Count; i++)
            layersRectangles.Add(new Rectangle(startingX + 20, startingY + (i * 35) + 20, width - 40, 32));
    }

    private void AddLayer()
    {
        layers.Add("new layer");
        UpdateLayerReacts();
    }
}
