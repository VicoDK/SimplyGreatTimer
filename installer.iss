[Setup]
AppName=SimplyGreatTimer
AppVersion=1.0
DefaultDirName={pf}\SimplyGreatTimer
DefaultGroupName=SimplyGreatTimer
OutputDir=Output
OutputBaseFilename=SimplyGreatTimer-Windows-Install
Compression=lzma
SolidCompression=yes

[Files]
Source: "publish\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\SimplyGreatTimer"; Filename: "{app}\MyApp.exe"
Name: "{commondesktop}\SimplyGreatTimer"; Filename: "{app}\MyApp.exe"
