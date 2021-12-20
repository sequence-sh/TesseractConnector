using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using Tesseract;

namespace Reductech.Sequence.Connectors.Tesseract;

/// <summary>
/// Settings for the Tesseract connector
/// </summary>
[Serializable]
public class TesseractSettings : IEntityConvertible
{
    /// <summary>
    /// Empty constructor for deserialization
    /// </summary>
    public TesseractSettings() { }

    /// <summary>
    /// The path to the TessData folder
    /// </summary>
    [JsonPropertyName("TessDataPath")]
    public string? TessDataPath { get; set; }

    /// <summary>
    /// Default Tesseract Settings
    /// </summary>
    public static readonly TesseractSettings Default = new();

    /// <summary>
    /// Gets the tesseract engine
    /// </summary>
    /// <returns></returns>
    public TesseractEngine GetEngine()
    {
        string tessDataPath;

        if (TessDataPath is not null)
            tessDataPath = TessDataPath;
        else
        {
            var assembly = Assembly.GetAssembly(typeof(TesseractOCR))!;
            tessDataPath = Path.Combine(assembly.Location, "..", "tessData");
        }

        var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default);
        return engine;
    }
}
