using System.Text.RegularExpressions;
using Reductech.Sequence.Core.Internal.Errors;

namespace Reductech.Sequence.Connectors.Tesseract;

/// <summary>
/// Gets the image format implied by the file extension of the file name.
/// Note that this does not actually look at the file.
/// </summary>
public class GetImageFormat : CompoundStep<SCLEnum<ImageFormat>>
{
    /// <summary>
    /// The name of the image file
    /// </summary>
    [Required]
    [StepProperty(1)]
    public IStep<StringStream> FileName { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task<Result<SCLEnum<ImageFormat>, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var fileName =
            await FileName.Run(stateMonad, cancellationToken).Map(x => x.GetStringAsync());

        if (fileName.IsFailure)
            return fileName.ConvertFailure<SCLEnum<ImageFormat>>();

        var match = FormatRegex.Match(fileName.Value);

        if (match.Success)
        {
            var result = ConvertString(match.Groups["extension"].Value.ToLowerInvariant());

            return new SCLEnum<ImageFormat>(result);
        }
        else
        {
            return new SCLEnum<ImageFormat>(ImageFormat.Default);
        }
    }

    private static readonly Regex FormatRegex =
        new(@"\A(.*\.)?(?<extension>[a-z]+)\Z", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Convert a string to an ImageFormat enum.
    /// </summary>
    public static ImageFormat ConvertString(string s)
    {
        var result = s.ToLowerInvariant()
            switch
            {
                "jpg"  => ImageFormat.JPG,
                "jpeg" => ImageFormat.JPG,
                "png"  => ImageFormat.Png,
                "bmp"  => ImageFormat.Bmp,
                "gif"  => ImageFormat.GIF,
                "tif"  => ImageFormat.Tif,
                "tiff" => ImageFormat.Tif,
                _      => ImageFormat.Default
            };

        return result;
    }

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<GetImageFormat, SCLEnum<ImageFormat>>();
}
