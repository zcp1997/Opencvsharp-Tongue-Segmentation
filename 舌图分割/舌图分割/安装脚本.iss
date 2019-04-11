; 脚本用 Inno Setup 脚本向导 生成。
; 查阅文档获取创建 INNO SETUP 脚本文件的详细资料！

#define MyAppName "数字化舌图分割处理系统"
#define MyAppVersion "1.0"
#define MyAppPublisher "南京中医药大学信息技术学院"
#define MyAppExeName "舌图分割.exe"

[Setup]
; 注意: AppId 的值是唯一识别这个程序的标志。
; 不要在其他程序中使用相同的 AppId 值。
; (在编译器中点击菜单“工具 -> 产生 GUID”可以产生一个新的 GUID)
AppId={{553E90E6-A5B3-42F7-B6B8-02F62424B6C6}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=C:\Users\dev2\Desktop
OutputBaseFilename=setup
SetupIconFile=C:\Users\dev2\Desktop\工作\毕设相关test\舌图分割\舌图分割\ooopic_1553754152.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Languages\ChineseSimp.isl"
Name: "english"; MessagesFile: "compiler:Languages\English.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\Users\dev2\Desktop\工作\毕设相关test\舌图分割\舌图分割\bin\Debug\舌图分割.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\dev2\Desktop\工作\毕设相关test\舌图分割\舌图分割\bin\Debug\IrisSkin4.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\dev2\Desktop\工作\毕设相关test\舌图分割\舌图分割\bin\Debug\OpenCvSharp.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\dev2\Desktop\工作\毕设相关test\舌图分割\舌图分割\bin\Debug\OpenCvSharp.dll.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\dev2\Desktop\工作\毕设相关test\舌图分割\舌图分割\bin\Debug\OpenCvSharp.Extensions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\dev2\Desktop\工作\毕设相关test\舌图分割\舌图分割\bin\Debug\OpenCvSharpExtern.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\dev2\Desktop\工作\毕设相关test\舌图分割\舌图分割\bin\Debug\RealOne.ssk"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\dev2\Desktop\工作\毕设相关test\舌图分割\舌图分割\bin\Debug\舌图分割.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\dev2\Desktop\工作\毕设相关test\舌图分割\舌图分割\bin\Debug\舌图分割.pdb"; DestDir: "{app}"; Flags: ignoreversion
; 注意: 不要在任何共享的系统文件使用 "Flags: ignoreversion"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
