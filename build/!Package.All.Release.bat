%~dp0

rd/s/q ../_output/

cd ../

dotnet msbuild ./AtomUI.sln /p:Configuration=Release

cd packages/AtomUI/

dotnet msbuild ./AtomUI.csproj /p:Configuration=Release

pause