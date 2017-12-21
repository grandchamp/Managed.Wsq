# Managed.Wsq

WSQ extractor for .NET Core. I've adapted the code from https://www.codeproject.com/Tips/429481/WSQ-to-BMP-converter to .NET Core.

# Usage

```
IWsqDecoder wsq = new WsqDecoder();
var bmp = wsq.Decode(wsqByteArray);
```
