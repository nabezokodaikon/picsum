@SET PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin";%PATH%
@SET OUTPUT_PATH="%CD%\bin_release"
@SET CONFIGURATION=Release

RD /s /q "%OUTPUT_PATH%"

MSBuild src\PicSum.sln /p:OutputPath="%OUTPUT_PATH%" /t:Clean;Rebuild /p:Configuration=%CONFIGURATION%;Platform="Any CPU"

DEL "%OUTPUT_PATH%\*.pdb"
DEL "%OUTPUT_PATH%\*.xml"

PAUSE
