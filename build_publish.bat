@SET PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin";%PATH%
@SET OUTPUT_PATH=%CD%\bin_

dotnet restore src\PicSum.Main\PicSum.Main.csproj

MSBuild src\PicSum.Main\PicSum.Main.csproj /t:Publish ^
  /p:Configuration=Release ^
  /p:OutputPath="%OUTPUT_PATH%" ^
  /p:Platform="x64"

RD /S /Q "%OUTPUT_PATH%"
