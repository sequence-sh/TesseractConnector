using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Reductech.Sequence.Core.Internal.Errors;
using Tesseract;

namespace Reductech.Sequence.Connectors.Tesseract;

/// <summary>
/// Returns true if the file in the specified path exists, false otherwise
/// </summary>
public class TesseractOCR : CompoundStep<StringStream>
{
    /// <summary>
    /// The image data to OCR
    /// </summary>
    [StepProperty(1)]
    [Required]
    [Alias("File")]
    [Log(LogOutputLevel.None)]
    public IStep<StringStream> ImageData { get; set; } = null!;

    /// <summary>
    /// The format of the image
    /// </summary>
    [StepProperty(2)]
    [DefaultValueExplanation("Default")]
    [Alias("Format")]
    [Log(LogOutputLevel.Trace)]
    public IStep<SCLEnum<ImageFormat>> ImageFormat { get; set; } =
        new SCLConstant<SCLEnum<ImageFormat>>(
            new SCLEnum<ImageFormat>(Tesseract.ImageFormat.Default)
        );

    private static byte[] GetByteArray(StringStream ss)
    {
        var stream = ss.GetStream().stream;

        if (stream is MemoryStream ms1)
            return ms1.ToArray();

        using MemoryStream ms2 = new();
        stream.CopyTo(ms2);
        return ms2.ToArray();
    }

    /// <inheritdoc />
    protected override async Task<Result<StringStream, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var dataResult = await ImageData.Run(stateMonad, cancellationToken).Map(GetByteArray);

        if (dataResult.IsFailure)
            return dataResult.ConvertFailure<StringStream>();

        var formatResult = await ImageFormat.Run(stateMonad, cancellationToken);

        if (formatResult.IsFailure)
            return formatResult.ConvertFailure<StringStream>();

        var tesseractSettings = SettingsHelpers.TryGetTesseractSettings(stateMonad.Settings)
            .ToMaybe()
            .GetValueOrDefault(TesseractSettings.Default);

        string resultText;

        try
        {
            using var engine = tesseractSettings.GetEngine();

            Pix? data;

            if (formatResult.Value == Tesseract.ImageFormat.Tif)
                data = Pix.LoadTiffFromMemory(dataResult.Value);
            else
            {
                data = Pix.LoadFromMemory(dataResult.Value);
            }

            using var page = engine.Process(data);

            var text = page.GetText();

            stateMonad.Logger.LogInformation(
                "Performing OCR",
                page.GetMeanConfidence()
            ); //TODO use proper logging

            stateMonad.Logger.LogInformation(
                "Mean confidence: {Confidence}",
                page.GetMeanConfidence()
            );

            stateMonad.Logger.LogInformation("Text (GetText): \r\n{OCRText}", text);
            stateMonad.Logger.LogInformation("Text (iterator):");

            var sb = new StringBuilder();

            using var iterator = page.GetIterator();

            iterator.Begin();

            do
            {
                do
                {
                    do
                    {
                        do
                        {
                            if (iterator.IsAtBeginningOf(PageIteratorLevel.Block))
                            {
                                //sb.AppendLine("<BLOCK>");
                            }

                            sb.Append(iterator.GetText(PageIteratorLevel.Word));
                            sb.Append(' ');

                            if (iterator.IsAtFinalOf(
                                    PageIteratorLevel.TextLine,
                                    PageIteratorLevel.Word
                                ))
                            {
                                sb.AppendLine();
                            }
                        } while (iterator.Next(
                                     PageIteratorLevel.TextLine,
                                     PageIteratorLevel.Word
                                 ));

                        if (iterator.IsAtFinalOf(
                                PageIteratorLevel.Para,
                                PageIteratorLevel.TextLine
                            ))
                        {
                            sb.AppendLine();
                        }
                    } while (iterator.Next(
                                 PageIteratorLevel.Para,
                                 PageIteratorLevel.TextLine
                             ));
                } while (iterator.Next(
                             PageIteratorLevel.Block,
                             PageIteratorLevel.Para
                         ));
            } while (iterator.Next(PageIteratorLevel.Block));

            resultText = sb.ToString().Trim();
        }
        catch (Exception e)
        {
            var error = ErrorCode.Unknown.ToErrorBuilder(e.GetFullMessage()).WithLocation(this);
            return Result.Failure<StringStream, IError>(error);
        }

        return new StringStream(resultText);
    }

    /// <inheritdoc />
    public override IStepFactory StepFactory => TesseractOCRStepFactory.Instance;

    private class TesseractOCRStepFactory : SimpleStepFactory<TesseractOCR, StringStream>
    {
        private TesseractOCRStepFactory() { }

        public static SimpleStepFactory<TesseractOCR, StringStream> Instance { get; } =
            new TesseractOCRStepFactory();

        /// <inheritdoc />
        public override IEnumerable<Type> ExtraEnumTypes
        {
            get
            {
                yield return typeof(ImageFormat);
            }
        }
    }
}
