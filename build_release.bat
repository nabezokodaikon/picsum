@SET PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin";%PATH%
@SET OUTPUT_PATH=%CD%\bin_release

RD /S /Q "%OUTPUT_PATH%"

dotnet restore src\PicSum.sln

MSBuild src\PicSum.sln /t:Rebuild ^
  /p:Configuration=Release ^
  /p:DebugType=None ^
  /p:OutputPath="%OUTPUT_PATH%" ^
  /p:Platform="Any CPU"

DEL "%OUTPUT_PATH%\NLog.debug.config"
DEL "%OUTPUT_PATH%\*.pdb"

RENAME "%OUTPUT_PATH%\picsum.runtimeconfig.json" "temp"
DEL "%OUTPUT_PATH%\*.runtimeconfig.json"
RENAME "%OUTPUT_PATH%\temp" "picsum.runtimeconfig.json"
