@SET PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin";%PATH%
@SET OUTPUT_PATH=%CD%\bin_release

RMDIR /s /q %OUTPUT_PATH%

MSBuild src\PicSum.sln /t:Rebuild ^
  /p:OutputPath="%OUTPUT_PATH%" ^
  /p:Configuration=Release ^
  /p:Platform="Any CPU" ^
  /p:DebugType=None ^
  /p:GenerateDependencyFile=false

DEL "%OUTPUT_PATH%\NLog.debug.config"
DEL "%OUTPUT_PATH%\*.pdb"

RENAME "%OUTPUT_PATH%\picsum.runtimeconfig.json" "temp"
DEL "%OUTPUT_PATH%\*.runtimeconfig.json"
RENAME "%OUTPUT_PATH%\temp" "picsum.runtimeconfig.json"

PAUSE
