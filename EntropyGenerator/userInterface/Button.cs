using System.Numerics;
using Raylib_cs;

namespace EntropyGenerator.userInterface;

public class Button
{
    public Color BackgroundColor;
    public Rectangle Box;
    public string Label;
    public Color TextColor;
    public Font Font;

    public Button(float x, float y, float width, float height, string label = "", Color bgColor = default,
        Color textColor = default)
    {
        Box = new Rectangle(x, y, width, height);
        Label = label;
        BackgroundColor = bgColor.Equals(default(Color)) ? Color.Gray : bgColor;
        TextColor = textColor.Equals(default(Color)) ? Color.White : textColor;
    }

    public virtual void Draw()
    {
        Raylib.DrawRectangleRec(Box, BackgroundColor);
        if (!string.IsNullOrEmpty(Label))
        {
            var textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), Label, 16, 1);
            var textPos = new Vector2(Box.X + (Box.Width - textSize.X) / 2, Box.Y + (Box.Height - textSize.Y) / 2);
            Raylib.DrawTextEx(Font, Label, new Vector2(textPos.X, textPos.Y), 16, 1, TextColor);
        }
    }

    public bool IsClicked()
    {
        return Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), Box) &&
               Raylib.IsMouseButtonReleased(MouseButton.Left);
    }
}