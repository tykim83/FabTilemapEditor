using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

//TODO: Need to unload the Texture2D
public class Layers
{
    const int PANEL_X = 0;
    const int PANEL_Y = 700;
    const int PANEL_WIDTH = 600;
    const int PANEL_HEIGHT = 380;

    private Texture2D gearIcon;
    private TextButton? button;

    // Layers fields
    private int activeLayer = 0;
    private List<LayerPanel> layersPanels = [];

    // Drag fields
    private int? draggingLayerIndex = null;
    private float dragOffsetY = 0;
    private float dragStartTime = 0f;
    private bool isDragging = false;
    private const float DRAG_DELAY = 0.20f;

    public void GameStartup()
    {
        // Init Button
        var availableSpace = Utilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Layers");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;
        var height = (int)availableSpace.Height;
        button = new TextButton(startingX + 10, startingY + height - 50, 130, 30, "Add Layer", AddLayer);

        // Load Icons
        Image image = Raylib.LoadImage("Assets/gear_icon.png");
        Raylib.ImageResize(ref image, 28, 28);
        gearIcon = Raylib.LoadTextureFromImage(image);
        Raylib.UnloadImage(image);

        // Init Layers
        InitLayerRect();
    }

    public void HandleInput()
    {
        button?.Update();

        foreach (var layerPanel in layersPanels)
            layerPanel.Update();

        Vector2 mousePos = Raylib.GetMousePosition();

        // Init Drag Layer with Delay
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            for (int i = 0; i < layersPanels.Count; i++)
            {
                if (Raylib.CheckCollisionPointRec(mousePos, layersPanels[i].Rect))
                {
                    draggingLayerIndex = i;
                    dragOffsetY = mousePos.Y - layersPanels[i].Rect.Y;
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
            var draggedRect = layersPanels[index].Rect;
            draggedRect.Y = mousePos.Y - dragOffsetY;
            layersPanels[index].Rect = draggedRect;

            // Swap layers
            for (int i = 0; i < layersPanels.Count; i++)
            {
                if (i != index)
                {
                    float halfHeight = layersPanels[i].Rect.Height / 2;
                    float centerY = layersPanels[i].Rect.Y + halfHeight;

                    if ((index > i && layersPanels[index].Rect.Y < centerY) ||
                        (index < i && layersPanels[index].Rect.Y + layersPanels[index].Rect.Height > centerY))
                    {
                        if (activeLayer == i)
                            activeLayer = index;
                        else if (activeLayer == index)
                            activeLayer = i;

                        (layersPanels[index].Index, layersPanels[i].Index) = (i, index);
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
                for (int i = 0; i < layersPanels.Count; i++)
                {
                    if (Raylib.CheckCollisionPointRec(mousePos, layersPanels[i].Rect))
                    {
                        layersPanels[activeLayer].ToggleActive();
                        layersPanels[i].ToggleActive();
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
        Utilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Layers");

        foreach (var layerPanel in layersPanels)
            layerPanel.Draw();

        button?.Draw();
    }

    private void Test() => Console.WriteLine("Click");

    private void InitLayerRect()
    {
        layersPanels.Clear();

        var availableSpace = Utilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Layers");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;

        var rect = new Rectangle(startingX + 20, startingY + 20, width - 40, 32);
        var layerPanel = new LayerPanel(rect, "Background", 0, Test, gearIcon);
        layerPanel.ToggleActive();

        layersPanels.Add(layerPanel);
    }

    private void UpdateLayerReacts()
    {
        var availableSpace = Utilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Layers");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;

        layersPanels = [.. layersPanels.OrderBy(c => c.Index)];

        for (var i = 0; i < layersPanels.Count; i++)
        {
            layersPanels[i].Rect = new Rectangle(startingX + 20, startingY + (i * 35) + 20, width - 40, 32);
        }
    }

    private void AddLayer()
    {
        var layerPanel = new LayerPanel(new Rectangle(), "new layer", layersPanels.Count, Test, gearIcon);
        layersPanels.Add(layerPanel);
        UpdateLayerReacts();
    }
}
