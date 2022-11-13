namespace Sequence.Connectors.Tesseract.Tests;

public partial class GetImageFormatTests : StepTestBase<GetImageFormat, SCLEnum<ImageFormat>>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            static StepCase GetStepCase(string fName, ImageFormat expected)
            {
                return new(
                    $"{fName} - {expected}",
                    new GetImageFormat() { FileName = StaticHelpers.Constant(fName) },
                    new SCLEnum<ImageFormat>(expected)
                );
            }

            yield return GetStepCase("",          ImageFormat.Default);
            yield return GetStepCase("blah",      ImageFormat.Default);
            yield return GetStepCase("blah.blah", ImageFormat.Default);
            yield return GetStepCase("a.jpg",     ImageFormat.JPG);
            yield return GetStepCase("jpg",       ImageFormat.JPG);
            yield return GetStepCase("jpeg",      ImageFormat.JPG);
            yield return GetStepCase(".jpg",      ImageFormat.JPG);
            yield return GetStepCase("gif",       ImageFormat.GIF);
            yield return GetStepCase("tif",       ImageFormat.Tif);
            yield return GetStepCase("tiff",      ImageFormat.Tif);
            yield return GetStepCase("bmp",       ImageFormat.Bmp);
            yield return GetStepCase("png",       ImageFormat.Png);
        }
    }
}
