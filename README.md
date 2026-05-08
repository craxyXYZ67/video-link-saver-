# Video Link Saver

A fully offline Windows desktop application to store and search YouTube & Instagram video links locally on your PC.

---

## Features

- Save video links with Platform, Channel Name, Purpose, and Category
- Live search with stacking filters — results refresh instantly as you type
- Automatic Excel file creation and self-healing (recreates if deleted/corrupted)
- Light and Dark mode with theme preference remembered across sessions
- Opens saved links in your default browser
- 100% offline — no internet, no telemetry, no cloud

---

## Tech Stack

| Area | Technology |
|------|-----------|
| Language | C# 12 |
| Framework | .NET 8 |
| UI | WPF (Windows Presentation Foundation) |
| Architecture | MVVM (Model-View-ViewModel) |
| Excel library | ClosedXML (MIT license) |
| Build tool | Visual Studio 2022 / dotnet CLI |

---

## Data Storage Location

The Excel file is stored at:

```
%LOCALAPPDATA%\VideoLinkSaver\the saves links.xlsx
```

For most users this resolves to:

```
C:\Users\<YourName>\AppData\Local\VideoLinkSaver\the saves links.xlsx
```

The app:
- Creates this folder and file automatically on first launch
- Recreates the file automatically if it is deleted, renamed, moved, or corrupted
- Never crashes if the file is missing

---

## Requirements

- Windows 10 or Windows 11 (x64 or x86)
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (or install via Visual Studio)
- Visual Studio 2022 (Community, Professional, or Enterprise)

---

## How to Build

### Option A — Visual Studio 2022 (recommended)

1. Open `VideoLinkSaver.sln` (or open the folder and let VS detect the `.csproj`)
2. Make sure the **.NET desktop development** workload is installed in Visual Studio Installer
3. Click **Build → Build Solution** (or press `Ctrl+Shift+B`)
4. Run with **F5** (debug) or **Ctrl+F5** (without debugger)

### Option B — dotnet CLI

```bash
# Restore NuGet packages
dotnet restore VideoLinkSaver.csproj

# Build
dotnet build VideoLinkSaver.csproj -c Release

# Run directly
dotnet run --project VideoLinkSaver.csproj
```

---

## Compiling to a Standalone .exe

### Single-file self-contained executable (recommended for sharing)

Run this command inside the `VideoLinkSaver/` folder:

```bash
dotnet publish VideoLinkSaver.csproj ^
  -c Release ^
  -r win-x64 ^
  --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:IncludeNativeLibrariesForSelfExtract=true ^
  -o publish\
```

The output will be at `publish\VideoLinkSaver.exe` — a single file you can copy anywhere on a Windows machine without needing .NET installed separately.

For 32-bit Windows, change `-r win-x64` to `-r win-x86`.

---

## Creating an Installer (optional)

### Using Inno Setup (free, open-source)

1. Download and install [Inno Setup](https://jrsoftware.org/isinfo.php)
2. Create a script file `installer.iss` with the following content:

```ini
[Setup]
AppName=Video Link Saver
AppVersion=1.0.0
DefaultDirName={autopf}\VideoLinkSaver
DefaultGroupName=Video Link Saver
OutputDir=installer
OutputBaseFilename=VideoLinkSaverSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Files]
Source: "publish\VideoLinkSaver.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\Video Link Saver"; Filename: "{app}\VideoLinkSaver.exe"
Name: "{commondesktop}\Video Link Saver"; Filename: "{app}\VideoLinkSaver.exe"

[Run]
Filename: "{app}\VideoLinkSaver.exe"; Description: "Launch Video Link Saver"; Flags: nowait postinstall skipifsilent
```

3. Open `installer.iss` in Inno Setup and click **Build → Compile**
4. Your installer will be at `installer\VideoLinkSaverSetup.exe`

---

## NuGet Packages

| Package | Version | License | Purpose |
|---------|---------|---------|---------|
| ClosedXML | 0.102.2 | MIT | Read/write Excel (.xlsx) files |

All other dependencies are part of the .NET 8 SDK and WPF framework — no additional packages needed.

---

## Project Structure

```
VideoLinkSaver/
├── VideoLinkSaver.csproj       # Project file — defines .NET 8 target, NuGet refs
├── App.xaml                    # Application entry, global resource dictionary
├── App.xaml.cs                 # App startup, global exception handling
├── MainWindow.xaml             # Shell window: app bar + ContentControl for navigation
├── MainWindow.xaml.cs
│
├── Models/
│   └── VideoLink.cs            # Data model matching Excel columns
│
├── ViewModels/
│   ├── BaseViewModel.cs        # INotifyPropertyChanged base
│   ├── RelayCommand.cs         # ICommand implementation for MVVM
│   ├── MainViewModel.cs        # Root VM: navigation + shared services
│   ├── HomeViewModel.cs        # Home screen commands
│   ├── CreateLinkViewModel.cs  # Form input, validation, save logic
│   ├── SearchViewModel.cs      # Filter logic, live filtering, result list
│   └── DataLocationViewModel.cs # File path display, open folder
│
├── Views/
│   ├── HomeView.xaml / .cs     # Home screen UI
│   ├── CreateLinkView.xaml / .cs  # Create Link form UI
│   ├── SearchView.xaml / .cs   # Search & filter UI
│   └── DataLocationView.xaml / .cs # Data location info UI
│
├── Services/
│   ├── ExcelService.cs         # All Excel file operations (ClosedXML)
│   └── ThemeService.cs         # Light/Dark theme switching + persistence
│
├── Converters/
│   └── BoolToVisibilityConverter.cs  # WPF value converters for the UI
│
├── Themes/
│   ├── LightTheme.xaml         # Light mode colors and brushes
│   └── DarkTheme.xaml          # Dark mode colors and brushes
│
└── README.md                   # This file
```

---

## Architecture Notes

- **MVVM pattern**: Views are pure XAML with no logic. All behavior is in ViewModels.
- **Navigation**: `MainViewModel.CurrentView` holds the active ViewModel; WPF `DataTemplate` entries in `MainWindow.xaml` map each ViewModel type to its View automatically.
- **Theme system**: Themes are XAML `ResourceDictionary` files swapped at runtime by `ThemeService`. All colors reference `DynamicResource` keys so they update instantly without restarting views.
- **Excel self-healing**: `ExcelService.EnsureFileExists()` is called before every read/write operation. If the file is missing or corrupt, it is recreated automatically.
- **No external runtime dependencies** beyond .NET 8 and ClosedXML.

---

## Privacy & Security

- No internet connection is ever made
- No telemetry, analytics, or tracking of any kind
- All data lives exclusively in `%LOCALAPPDATA%\VideoLinkSaver\`
- The application can be used in air-gapped (fully offline) environments
