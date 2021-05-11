using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Tesseract;

namespace Reductech.EDR.Connectors.Tesseract
{

/// <summary>
/// Returns true if the file in the specified path exists, false otherwise
/// </summary>
public class TesseractOCR : CompoundStep<StringStream>
{
    ///// <summary>
    ///// The path to the file to check.
    ///// </summary>
    //[StepProperty(1)]
    //[Required]
    //[Alias("File")]
    //[Log(LogOutputLevel.Trace)]
    //public IStep<StringStream> Path { get; set; } = null!;

    [StepProperty(1)]
    [Required]
    [Alias("File")]
    [Log(LogOutputLevel.Trace)]
    public IStep<StringStream> ImageData { get; set; } = null!;

    private static byte[] GetByteArray(StringStream ss)
    {
        using MemoryStream ms = new();

        //todo if stream is a memory stream
        ss.GetStream().stream.CopyTo(ms);
        return ms.ToArray();
    }

    /// <inheritdoc />
    protected override async Task<Result<StringStream, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        //var pathResult = await Path.Run(stateMonad, cancellationToken)
        //    .Map(async x => await x.GetStringAsync());

        //if (pathResult.IsFailure)
        //    return pathResult.ConvertFailure<StringStream>();

        var dataResult = await ImageData.Run(stateMonad, cancellationToken).Map(GetByteArray);

        if (dataResult.IsFailure)
            return dataResult.ConvertFailure<StringStream>();

        string resultText;

        try
        {
            using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);

            var data = Pix.LoadTiffFromMemory(dataResult.Value);

            //using var img = Pix.LoadFromFile(pathResult.Value);

            using var page = engine.Process(data);

            var text = page.GetText();

            stateMonad.Logger.LogInformation("Mean confidence: {0}", page.GetMeanConfidence());

            stateMonad.Logger.LogInformation("Text (GetText): \r\n{0}", text);
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
            var error = ErrorCode.Unknown.ToErrorBuilder(e.Message).WithLocation(this);
            return Result.Failure<StringStream, IError>(error);
        }

        return new StringStream(resultText);
    }

    /// <inheritdoc />
    public override IStepFactory StepFactory => new SimpleStepFactory<TesseractOCR, StringStream>();
}

}
