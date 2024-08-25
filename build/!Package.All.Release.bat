%~dp0

rd/s/q ..\_output\

cd ..\

dotnet msbuild .\AtomUI.sln /p:Configuration=Release

pause