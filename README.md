# DIL

A Minecraft launcher built with WPF and .NET 10.

## Project Structure

```
DIL/
├── DILApp/          # WPF application (launcher UI)
├── DILCore/         # Core library (launcher engine)
├── DIL.sln          # Visual Studio solution file
├── LICENSE          # MIT License
└── README.md        # This file
```

## Build

```bash
dotnet restore DIL.sln
dotnet build DIL.sln
```

## Publish

```bash
dotnet publish DILApp/DILApp.csproj -c Release -r win-x64
```
## Thanks
corona studio: projbobcat
PCL2: some of xaml

## License

This project is licensed under the [MIT License](LICENSE).