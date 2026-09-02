using System;

namespace DILCore.Exceptions;

public class CurseForgeAddonResolveException(string? message) : Exception(message);