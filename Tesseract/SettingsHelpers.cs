using CSharpFunctionalExtensions;
using Reductech.EDR.ConnectorManagement.Base;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Entity = Reductech.EDR.Core.Entity;

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
    public static Result<TesseractSettings, IErrorBuilder> TryGetTesseractSettings(Entity settings)
    {
        var connectorSettings = settings.TryGetValue(
            new EntityPropertyKey(
                StateMonad.ConnectorsKey,
                "Reductech.EDR.Connectors.Tesseract",
                nameof(ConnectorSettings.Settings)
            )
        );

        if (connectorSettings.HasNoValue
         || connectorSettings.GetValueOrThrow().ObjectValue is not Entity ent)
            return ErrorCode.MissingStepSettings.ToErrorBuilder(
                "Reductech.EDR.Connectors.Tesseract"
            );

        var settingsObj = EntityConversionHelpers.TryCreateFromEntity<TesseractSettings>(ent);

        return settingsObj;
    }
}

}
