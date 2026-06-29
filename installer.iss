[Setup]
AppName=SimplyGreatTimer
AppVersion=1.0
DefaultDirName={pf}\SimplyGreatTimer
DefaultGroupName=SimplyGreatTimer
OutputDir=Output
OutputBaseFilename=SimplyGreatTimer-Setup
Compression=lzma
SolidCompression=yes

[Files]
Source: "bin\Release\win-x64\publish\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\SimplyGreatTimer"; Filename: "{app}\SimplyGreatTimer.exe"