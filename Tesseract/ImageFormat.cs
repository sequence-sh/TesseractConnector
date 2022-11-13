namespace Sequence.Connectors.Tesseract;

/// <summary>
/// The format of the image to OCR.
/// </summary>
public enum ImageFormat
{
    /// <summary>
    /// Image format not specified - this may result in failure.
    /// </summary>
    Default,

    /// <summary>
    /// Portable Network Graphics
    /// </summary>
    Png,

    /// <summary>
    /// Bitmap image file
    /// </summary>
    Bmp,

    /// <summary>
    /// Tag Image File Format - TIFF or TIF
    /// </summary>
    Tif,

    /// <summary>
    /// Joint Photographic Experts Group - JPG or JPEG
    /// </summary>
    JPG,

    /// <summary>
    /// Graphics Interchange Format
    /// </summary>
    GIF
}
