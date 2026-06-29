[Setup]
AppName=SimplyGreatTimer
AppVersion=1.0
DefaultDirName={autopf}\SimplyGreatTimer
DefaultGroupName=SimplyGreatTimer
ArchitecturesInstallIn64BitMode=x64compatible
OutputDir=Output
OutputBaseFilename=SimplyGreatTimer-Setup
Compression=lzma
SolidCompression=yes

[Files]
Source: "publish\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\SimplyGreatTimer"; Filename: "{app}\SimplyGreatTimer.exe"
Name: "{group}\Uninstall SimplyGreatTimer"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\SimplyGreatTimer.exe"; Description: "Launch SimplyGreatTimer"; Flags: nowait postinstall skipifsilent
