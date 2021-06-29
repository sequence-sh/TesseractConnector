using System.Collections.Generic;
using System.Reflection;
using Reductech.EDR.ConnectorManagement.Base;
using Reductech.EDR.Connectors.FileSystem;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.TestHarness;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.Tesseract.Tests
{

public partial class TesseractOCRTests : StepTestBase<TesseractOCR, StringStream>
{
    public static StepFactoryStore CreateTestStepFactoryStore()
    {
        var tesseractAssembly  = Assembly.GetAssembly(typeof(TesseractOCR))!;
        var fileSystemAssembly = Assembly.GetAssembly(typeof(FileRead))!;

        var stepFactoryStore =
            StepFactoryStore.CreateFromAssemblies(tesseractAssembly, fileSystemAssembly);

        return stepFactoryStore;
    }

    public const string ExpectedFileText = @"This is a lot of 12 point text to test the
ocr code and see if it works on all types
of file format.

The quick brown dog jumped over the
lazy fox. The quick brown dog jumped
over the lazy fox. The quick brown dog
jumped over the lazy fox. The quick
brown dog jumped over the lazy fox.";

    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            var stepFactoryStore = CreateTestStepFactoryStore();

            yield return new StepCase(
                    "Check tif",
                    new TesseractOCR
                    {
                        ImageData = new FileRead
                        {
                            Path = new PathCombine { Paths = Array("phototest.tif") },
                        },
                        ImageFormat = Constant(ImageFormat.Tif)
                    },
                    ExpectedFileText
                ) { IgnoreLoggedValues = true, StepFactoryStoreToUse = stepFactoryStore }
                .WithContext(
                    ConnectorInjection.FileSystemKey,
                    new System.IO.Abstractions.FileSystem()
                );

            yield return new StepCase(
                    "Check bmp",
                    new TesseractOCR
                    {
                        ImageData = new FileRead
                        {
                            Path = new PathCombine { Paths = Array("phototest.bmp") }
                        },
                        ImageFormat = Constant(ImageFormat.Bmp)
                    },
                    ExpectedFileText
                ) { IgnoreLoggedValues = true, StepFactoryStoreToUse = stepFactoryStore }
                .WithContext(
                    ConnectorInjection.FileSystemKey,
                    new System.IO.Abstractions.FileSystem()
                );

            yield return new StepCase(
                    "Check png",
                    new TesseractOCR
                    {
                        ImageData = new FileRead
                        {
                            Path = new PathCombine { Paths = Array("phototest.png") }
                        },
                        ImageFormat = Constant(ImageFormat.Png)
                    },
                    ExpectedFileText
                ) { IgnoreLoggedValues = true, StepFactoryStoreToUse = stepFactoryStore }
                .WithContext(
                    ConnectorInjection.FileSystemKey,
                    new System.IO.Abstractions.FileSystem()
                );
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<ErrorCase> ErrorCases
    {
        get
        {
            var tesseractAssembly  = Assembly.GetAssembly(typeof(TesseractOCR))!;
            var fileSystemAssembly = Assembly.GetAssembly(typeof(FileRead))!;

            var tesseractData = new ConnectorData(
                new ConnectorSettings()
                {
                    Enable = true,
                    Id     = tesseractAssembly.GetName().Name!,
                    Settings = new Dictionary<string, object>()
                    {
                        { nameof(TesseractSettings.TessDataPath), "NotARealPath" }
                    }
                },
                tesseractAssembly
            );

            var fileSystemData = new ConnectorData(
                ConnectorSettings.DefaultForAssembly(fileSystemAssembly),
                fileSystemAssembly
            );

            var stepFactoryStore =
                StepFactoryStore.Create(fileSystemData, tesseractData);

            yield return new ErrorCase(
                    "Test Bad Settings",
                    new TesseractOCR
                    {
                        ImageData = new FileRead
                        {
                            Path = new PathCombine { Paths = Array("phototest.tif") },
                        },
                        ImageFormat = Constant(ImageFormat.Tif)
                    },
                    ErrorCode.Unknown.ToErrorBuilder(
                        "Failed to initialise tesseract engine.. See https://github.com/charlesw/tesseract/wiki/Error-1 for details."
                    )
                ) { IgnoreLoggedValues = true, StepFactoryStoreToUse = stepFactoryStore }
                .WithContext(
                    ConnectorInjection.FileSystemKey,
                    new System.IO.Abstractions.FileSystem()
                );

            foreach (var errorCase in base.ErrorCases)
            {
                yield return errorCase;
            }
        }
    }
}

}
