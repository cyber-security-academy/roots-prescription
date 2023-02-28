﻿using Microsoft.AspNetCore.Mvc;

namespace RootsPrescriptionWin.FileStorage;
public class FileStorageService : IFileStorageService
{
    private readonly ILogger<FileStorageService> _logger;
    private readonly string _filearchivepath = "FileArchive";

    public FileStorageService(ILogger<FileStorageService> logger)
    {
        _logger = logger;
    }

    public FileStream? GetFile(int id)
    {
        // TODO: Look up filename for the goven id
        string filename = "Prescription.pdf";
        return GetFile(filename);
    }

    public FileStream? GetFile(string filename)
    {
        string filepath= Path.Combine(_filearchivepath, filename);
        if (File.Exists(filepath))
        {
            _logger.LogInformation($"Downloading file: {filepath}");
            return System.IO.File.OpenRead(filepath);  // Use System.IO to avoid confusion
        } else
        {
            _logger.LogWarning($"Cannot find file file: {filepath}");
            return null;
        }
    }
}
