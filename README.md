# EDR Tesseract OCR Connector

[Reductech EDR](https://gitlab.com/reductech/edr) is a collection of
libraries that automates cross-application e-discovery and forensic workflows.

This connector contains steps to perform optical character recognition (OCR)
on image files. It uses the [Tesseract](https://github.com/tesseract-ocr/tesseract)
open source library as the OCR engine.

## Prerequisites

The following needs to be installed:

- [Microsoft Visual C++ Redistributable for Visual Studio 2015, 2017 and 2019](https://support.microsoft.com/en-us/topic/the-latest-supported-visual-c-downloads-2647da03-1eea-4433-9aff-95f26a218cc0)

## Examples

### OCR a bitmap image

```scala
- <path> = 'MyImage.bmp'
- <imageData> = FileRead <path>
- <imageFormat> = GetImageFormat <path>
- <imageText> = TesseractOCR <imageData> <imageFormat>
- Print <imageText>
```

### [Try Tesseract Connector](https://gitlab.com/reductech/edr/edr/-/releases)

Using [EDR](https://gitlab.com/reductech/edr/edr),
the command line tool for running Sequences.

## Documentation

Documentation is available here: https://docs.reductech.io

## E-discovery Reduct

The PowerShell Connector is part of a group of projects called
[E-discovery Reduct](https://gitlab.com/reductech/edr)
which consists of a collection of [Connectors](https://gitlab.com/reductech/edr/connectors)
and a command-line application for running Sequences, called
[EDR](https://gitlab.com/reductech/edr/edr/-/releases).

# Releases

Can be downloaded from the [Releases page](https://gitlab.com/reductech/edr/connectors/tesseract/-/releases).

# NuGet Packages

Are available in the [Reductech Nuget feed](https://gitlab.com/reductech/nuget/-/packages).
