using System;

namespace DILCore.Exceptions;

public class CurseForgeFileResolveException(string? message) : Exception(message);