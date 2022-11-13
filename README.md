# Sequence Tesseract OCR Connector

[Sequence®](https://sequence.sh) is a collection of libraries for
automation of cross-application e-discovery and forensic workflows.

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

# Documentation

https://sequence.sh

# Download

https://sequence.sh/download

# Try SCL and Core

https://sequence.sh/playground

# Package Releases

Can be downloaded from the [Releases page](https://gitlab.com/sequence/connectors/tesseract/-/releases).

# NuGet Packages

Release nuget packages are available from [nuget.org](https://www.nuget.org/profiles/Sequence).
