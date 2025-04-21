using System.Numerics;
using System.Reflection;
using EntropyGenerator.userInterface;
using Raylib_cs;
using TextCopy;

namespace EntropyGenerator;

internal static class Program
{
    private static Button generateButton;
    private static Button copyButton;
    private static CheckBox useSystemTime;
    private static CheckBox includeExtendedAscii;
    private static LineEdit outputLengthInput;
    private static LineEdit customSeedInput;
    private static TextBox previewBox;
    private static float backspaceHoldTime;
    private static bool backspaceHeld;
    private static List<char> printableAscii = new();
    private static List<char> extendedAscii = new();

    private static readonly char[] escapeChars = new[]
    {
        '\t',
        '\b',
        '\n',
        '\r',
        '\f',
        '\'',
        '\"',
        '\\',
        ' '
    };

    private static readonly int[] unusedChars = new[]
    {
        129,
        141,
        143,
        144,
        157,
        160,
        173
    };

    private static void Main()
    {
        Raylib.InitWindow(800, 600, "Entropy Generator");
        Raylib.SetTargetFPS(60);

        var fontPath = ExtractEmbeddedFont("EntropyGenerator.fonts.dos-vga-9x16.ttf", "dos-vga-9x16.ttf");
        var systemFont = Raylib.LoadFont(fontPath);
        Raylib.SetTextureFilter(systemFont.Texture, TextureFilter.Bilinear);

        generateButton = new Button(20, 310, 100, 30, "Generate", Color.Orange, Color.White);
        copyButton = new Button(130, 310, 100, 30, "Copy", Color.Blue, Color.White);
        useSystemTime = new CheckBox(350, 90, 20, 20, isChecked: true);
        customSeedInput = new LineEdit(350, 60, 100, 20, false);
        outputLengthInput = new LineEdit(350, 170, 100, 20) { Text = "12" };
        includeExtendedAscii = new CheckBox(350, 130, 20, 20);
        previewBox = new TextBox(20, 240, 760, 60, "", Color.LightGray, Color.Gray);
        var result = "";
        
        generateButton.Font = systemFont;
        copyButton.Font = systemFont;
        customSeedInput.Font = systemFont;
        outputLengthInput.Font = systemFont;

        printableAscii = GeneratePrintableAscii();
        extendedAscii = GenerateExtendedAscii();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            useSystemTime.Update();
            includeExtendedAscii.Update();

            useSystemTime.Draw();
            includeExtendedAscii.Draw();
            generateButton.Draw();
            copyButton.Draw();

            if (generateButton.IsClicked())
            {
                result = GenerateFinalOutput();
                previewBox.Text = result[..Math.Min(100, Convert.ToInt32(outputLengthInput.Text))];
            }

            if (copyButton.IsClicked()) CopyToClipboard(result);

            if (useSystemTime.IsClicked()) customSeedInput.CanEdit = !useSystemTime.IsChecked;

            var dt = Raylib.GetFrameTime();
            outputLengthInput.HandleInput(dt, ref backspaceHoldTime, ref backspaceHeld, 4, true, "8", 8, 1000);
            customSeedInput.HandleInput(dt, ref backspaceHoldTime, ref backspaceHeld, 19, false);

            Raylib.DrawTextEx(systemFont, "Entropy Generator", new Vector2(20, 20), 20, 1, Color.LightGray);

            Raylib.DrawTextEx(systemFont, "Custom Seed (Max 19 characters):", new Vector2(20, 60), 16, 1, Color.Gray);
            customSeedInput.Draw(Color.White, Color.White, 5, 2);

            Raylib.DrawTextEx(systemFont, "Use System Time:", new Vector2(20, 90), 16, 1, Color.Gray);

            Raylib.DrawTextEx(systemFont, "Include Extended ASCII (128-255):", new Vector2(20, 130), 16, 1, Color.Gray);

            Raylib.DrawTextEx(systemFont, "Output Length (8-1000):", new Vector2(20, 170), 16, 1, Color.Gray);
            outputLengthInput.Draw(Color.White, Color.White, 5, 2);

            Raylib.DrawTextEx(systemFont, "Live Preview (first 100 characters):", new Vector2(20, 210), 16, 1,
                Color.Gray);
            previewBox.Draw(systemFont);

            Raylib.EndDrawing();
        }

        Raylib.UnloadFont(systemFont);
        Raylib.CloseWindow();
    }

    private static string ExtractEmbeddedFont(string resourceName, string outputFileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName);
        var tempPath = Path.Combine(Path.GetTempPath(), outputFileName);
        using var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write);
        stream.CopyTo(fileStream);
        return tempPath;
    }

    private static List<char> GeneratePrintableAscii()
    {
        var Ascii = new List<char>();
        for (var i = 33; i < 123; i++) Ascii.Add((char)i);

        return Ascii;
    }

    private static List<char> GenerateExtendedAscii()
    {
        var Ascii = new List<char>();

        for (var i = 123; i < 256; i++)
        {
            if (unusedChars.Contains(i) || escapeChars.Contains((char)i)) continue;
            Ascii.Add((char)i);
        }

        return Ascii;
    }

    private static long GenerateSeed(int time)
    {
        var seed = "";

        var random1 = new Random(time);

        for (var i = 0; i < 4; i++)
        {
            var random2 = new Random(seed.GetHashCode());
            seed = "";
            for (var j = 0; j < random1.Next(10, 19); j++)
            {
                var number1 = random2.Next();
                var random3 = new Random(number1);
                seed += random3.Next(9);
            }
        }

        return Convert.ToInt64(seed);
    }

    private static string GenerateFinalOutput()
    {
        var charset = new List<char>(printableAscii);

        if (includeExtendedAscii.IsChecked)
            foreach (var c in extendedAscii)
                charset.Add(c);

        var seed = useSystemTime.IsChecked
            ? GenerateSeed(Environment.TickCount).GetHashCode()
            : customSeedInput.Text.GetHashCode();
        var rand = new Random(seed);

        var result = new char[Convert.ToInt32(outputLengthInput.Text)];
        for (var i = 0; i < Convert.ToInt32(outputLengthInput.Text); i++)
            result[i] = charset[rand.Next(charset.Count)];

        return new string(result);
    }

    private static void CopyToClipboard(string text)
    {
        ClipboardService.SetText(text);
    }
}