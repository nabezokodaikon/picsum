@ECHO OFF
ECHO „ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª
ECHO MsBuild‚ğÀs‚µ‚Ü‚·B
ECHO „ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª
@ECHO ON

@Set Path=C:\WINDOWS\Microsoft.NET\Framework64\v4.0.30319;%PATH%
MSBuild src/PicSum.sln /p:OutputPath="%CD%/bin" /t:Rebuild /p:Configuration=Release

pause