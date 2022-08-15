@echo off

echo Building for win-x64...
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false -o dist\win-x64> nul
echo done.
echo Building for linux-x64...
dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained false -o dist\linux-x64 > nul
echo done.
echo Building for osx-x64...
dotnet publish -c Release -r osx-x64 -p:PublishSingleFile=true --self-contained false -o dist\osx-x64 > nul
echo done.
