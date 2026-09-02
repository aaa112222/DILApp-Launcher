using System.Runtime.InteropServices;

namespace DILCore.Class.Model;

public record JavaRuntimeInfo(
    string JavaPath,
    OSPlatform JavaPlatform,
    Architecture JavaArch,
    bool UseSystemGlfwOnLinux,
    bool UseSystemOpenAlOnLinux);