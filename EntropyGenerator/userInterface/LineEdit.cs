using System.Numerics;
using Raylib_cs;

namespace EntropyGenerator.userInterface;

public class LineEdit
{
    public Rectangle Box;
    public bool CanEdit;
    private float caretBlinkTimer;
    private int caretPosition;
    public bool IsActive;
    private int scrollOffset;
    private bool showCaret = true;
    public string Text = "";
    private Color textDrawColor;
    public Font Font;
    private int fontSize = 16;

    public LineEdit(float x, float y, float width, float height, bool canEdit = true)
    {
        Box = new Rectangle(x, y, width, height);
        CanEdit = canEdit;
    }

    public void HandleInput(float deltaTime, ref float backspaceHoldTime, ref bool backspaceHeld, int maxLength = 19,
        bool digitsOnly = true, string fallback = "", int? minClamp = null, int? maxClamp = null)
    {
        if (!CanEdit) return;

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            IsActive = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), Box);

        if (IsActive)
        {
            var key = Raylib.GetCharPressed();
            while (key > 0)
            {
                if ((!digitsOnly || (key >= 32 && key <= 126)) && Text.Length < maxLength)
                    if (!digitsOnly || (key >= '0' && key <= '9'))
                    {
                        Text = Text.Insert(caretPosition, ((char)key).ToString());
                        caretPosition++;
                    }

                key = Raylib.GetCharPressed();
            }

            if (Raylib.IsKeyDown(KeyboardKey.Backspace))
            {
                backspaceHoldTime += deltaTime;
                backspaceHeld = true;

                if (backspaceHoldTime > 1f && Text.Length > 0 && caretPosition > 0)
                {
                    Text = Text.Remove(caretPosition - 1, 1);
                    caretPosition--;
                }
            }
            else if (Raylib.IsKeyReleased(KeyboardKey.Backspace))
            {
                if (backspaceHeld && Text.Length > 0 && caretPosition > 0)
                {
                    Text = Text.Remove(caretPosition - 1, 1);
                    caretPosition--;
                }

                backspaceHoldTime = 0f;
                backspaceHeld = false;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Left) && caretPosition > 0) caretPosition--;

            if (Raylib.IsKeyPressed(KeyboardKey.Right) && caretPosition < Text.Length) caretPosition++;

            if (string.IsNullOrEmpty(Text))
            {
                Text = fallback;
                caretPosition = Text.Length;
            }

            if (minClamp.HasValue && maxClamp.HasValue && int.TryParse(Text, out var value))
            {
                value = Math.Clamp(value, minClamp.Value, maxClamp.Value);
                Text = value.ToString();
                caretPosition = Math.Min(caretPosition, Text.Length);
            }

            var textWidth = Raylib.MeasureText(Text, 16);
            var caretXOffset = Raylib.MeasureText(Text[..Math.Min(caretPosition, Text.Length)], 16);

            if (caretXOffset > Box.Width - 10)
                scrollOffset = caretXOffset - (int)Box.Width + 10;
            else
                scrollOffset = 0;

            caretBlinkTimer += deltaTime;
            if (caretBlinkTimer >= 0.5f)
            {
                caretBlinkTimer = 0f;
                showCaret = !showCaret;
            }
        }
        else
        {
            caretPosition = Text.Length;
        }
    }

    public void Draw(Color borderColor, Color textColor, int textXOffset, int textYOffset)
    {
        Raylib.DrawRectangleLinesEx(Box, 1, IsActive ? Color.Yellow : borderColor);
        Raylib.BeginScissorMode((int)Box.X + 1, (int)Box.Y + 1, (int)Box.Width - 2, (int)Box.Height - 2);
        textDrawColor = CanEdit ? textColor : Color.DarkGray;

        Raylib.DrawTextEx(Font, Text, new Vector2(Box.X + textXOffset - scrollOffset, Box.Y + textYOffset), 16, 1, textDrawColor);
        
        string visibleText = Text.Substring(0, caretPosition);
        if (IsActive && showCaret)
        {
            var caretX = Box.X + textXOffset + Raylib.MeasureTextEx(Font, visibleText, fontSize, 1).X;
            Raylib.DrawRectangle((int)caretX, (int)Box.Y + textYOffset, 2, 16, textColor);
        }

        Raylib.EndScissorMode();
    }
}