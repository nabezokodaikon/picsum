@ECHO OFF
ECHO „ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª
ECHO MsBuild‚ğÀs‚µ‚Ü‚·B
ECHO „ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª„ª
@ECHO ON

MSBuild src/PicSum.sln /p:OutputPath="%CD%/bin" /t:Rebuild /p:Configuration=Release

pause