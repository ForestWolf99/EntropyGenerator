using System.Numerics;
using Raylib_cs;

namespace EntropyGenerator.userInterface;

public class TextBox
{
    public Color BorderColor;
    public Rectangle Box;
    public int Padding = 5;
    private Vector2 scrollOffset = new(0, 0);
    public string Text;
    public Color TextColor;

    public TextBox(float x, float y, float width, float height, string text, Color textColor, Color borderColor)
    {
        Box = new Rectangle(x, y, width, height);
        Text = text;
        TextColor = textColor;
        BorderColor = borderColor;
    }

    public void Draw(Font font)
    {
        Raylib.DrawRectangleLinesEx(Box, 1, BorderColor);
        Raylib.BeginScissorMode((int)Box.X + 1, (int)Box.Y + 1, (int)Box.Width - 2, (int)Box.Height - 2);

        var fontSize = 16;
        var lineSpacing = 2;
        var maxLineWidth = Box.Width - Padding * 2;
        var currentY = Box.Y + Padding;
        string[] words = Text.Split(' ');
        var line = "";

        foreach (var word in words)
        {
            var testLine = string.IsNullOrEmpty(line) ? word : line + " " + word;
            var textWidth = (int)Raylib.MeasureTextEx(font, testLine, fontSize, 1).X;
            if (textWidth <= maxLineWidth)
            {
                line = testLine;
            }
            else
            {
                //Raylib.DrawTextEx(font, line, new Vector2(Box.X + Padding, currentY), fontSize, 1, TextColor);
                Raylib.DrawText(line, (int)Box.X + Padding, (int)currentY, fontSize, TextColor);
                currentY += fontSize + lineSpacing;
                line = word;
            }
        }

        if (!string.IsNullOrEmpty(line))
        {
            //Raylib.DrawTextEx(font, line, new Vector2(Box.X + Padding, currentY), fontSize, 1, TextColor);
            Raylib.DrawText(line, (int)Box.X + Padding, (int)currentY, fontSize, TextColor);
        }
            

        Raylib.EndScissorMode();
    }
}