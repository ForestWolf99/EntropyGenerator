using Raylib_cs;

namespace EntropyGenerator.userInterface;

public class CheckBox : Button
{
    public bool IsChecked;

    public CheckBox(float x, float y, float width, float height, string label = "", bool isChecked = false)
        : base(x, y, width, height, label, Color.White, Color.Black)
    {
        IsChecked = isChecked;
    }

    public override void Draw()
    {
        var outerBox = new Rectangle(Box.X, Box.Y, Box.Width, Box.Height);
        var innerBox = new Rectangle(Box.X + 3, Box.Y + 3, Box.Width - 6, Box.Height - 6);
        Raylib.DrawRectangleRec(outerBox, Color.White);
        if (IsChecked) Raylib.DrawRectangleRec(innerBox, Color.Green);
    }

    public void Update()
    {
        if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), Box) &&
            Raylib.IsMouseButtonReleased(MouseButton.Left)) IsChecked = !IsChecked;
    }
}