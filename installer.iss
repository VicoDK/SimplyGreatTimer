[Setup]
AppPublisher=VicoDevelopment
AppName=SimplyGreatTimer
AppVersion=1.0
DefaultDirName={autopf}\My App
DefaultGroupName=My App
OutputBaseFilename=SimplyGreatTimer
Compression=lzma
SolidCompression=yes

[Files]
Source: "MyApp\Assets\Icon.ico"; DestDir: "{app}"
Source: "D:\SimplyGreatTimer\MyApp\bin\Release\net10.0\win-x64\publish\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Simply Great Timer"; Filename: "{app}\MyApp.exe"; IconFilename: "{app}\Icon.ico"
Name: "{commondesktop}\Simply Great Timer"; Filename: "{app}\MyApp.exe"; IconFilename: "{app}\Icon.ico"

[Run]
Filename: "{app}\MyApp.exe"; Description: "Launch Simply Great Timer"; Flags: nowait postinstall skipifsilent