@SET PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin";%PATH%
@SET OUTPUT_PATH=%CD%\bin_

dotnet restore src\PicSum.sln -p:PublishReadyToRun=true

MSBuild src\PicSum.sln /t:Publish ^
  /p:Configuration=Release ^
  /p:OutputPath="%OUTPUT_PATH%" ^
  /p:Platform="x64"

RD /S /Q "%OUTPUT_PATH%"
DEL /Q "%OUTPUT_PATH%publish\*.dll"
DEL /Q "%OUTPUT_PATH%publish\*.json"
RD /S /Q "%OUTPUT_PATH%publish\runtimes"
