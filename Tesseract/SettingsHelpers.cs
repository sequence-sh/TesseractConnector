using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;

namespace Reductech.EDR.Connectors.Tesseract
{

/// <summary>
/// Contains helper methods for Tesseract settings
/// </summary>
public static class SettingsHelpers
{
    /// <summary>
    /// Try to get a TesseractSettings from a list of Connector Informations
    /// </summary>
    public static Result<TesseractSettings, IErrorBuilder> TryGetTesseractSettings(
        IEnumerable<ConnectorSettings> connectorSettings)
    {
        var connectorInformation =
            connectorSettings.FirstOrDefault(
                x => x.Id.EndsWith("Tesseract", StringComparison.OrdinalIgnoreCase)
            );

        if (connectorInformation is null)
            return ErrorCode.MissingStepSettings.ToErrorBuilder("Tesseract");

        var nuixSettings =
            EntityConversionHelpers.TryCreateFromEntity<TesseractSettings>(
                connectorInformation.Settings
            );

        return nuixSettings;
    }
}

}
