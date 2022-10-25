using Reductech.Sequence.ConnectorManagement.Base;
using Reductech.Sequence.Core.Internal.Errors;
using Entity = Reductech.Sequence.Core.Entity;

namespace Reductech.Sequence.Connectors.Tesseract;

/// <summary>
/// Contains helper methods for Tesseract settings
/// </summary>
public static class SettingsHelpers
{
    /// <summary>
    /// Try to get a TesseractSettings from a list of Connector Information
    /// </summary>
    public static Result<TesseractSettings, IErrorBuilder> TryGetTesseractSettings(Entity settings)
    {
        var connectorSettings = settings.TryGetValue(
            new EntityNestedKey(
                StateMonad.ConnectorsKey,
                "Reductech.Sequence.Connectors.Tesseract",
                nameof(ConnectorSettings.Settings)
            )
        );

        if (connectorSettings.HasNoValue
         || connectorSettings.GetValueOrThrow() is not Entity ent)
            return ErrorCode.MissingStepSettings.ToErrorBuilder(
                "Reductech.Sequence.Connectors.Tesseract"
            );

        var settingsObj = EntityConversionHelpers.TryCreateFromEntity<TesseractSettings>(ent);

        return settingsObj;
    }
}
