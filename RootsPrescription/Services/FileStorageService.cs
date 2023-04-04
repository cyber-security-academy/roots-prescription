using Microsoft.AspNetCore.Mvc;
using RootsPrescription.Database;
using RootsPrescription.Models;

namespace RootsPrescription.FileStorage;
public class FileStorageService : IFileStorageService
{
    private readonly ILogger<FileStorageService> _logger;
    private readonly IDatabaseService _dbservice;
    private static readonly string _filearchivebasepath = "FileArchive";
    private static readonly string _filearchivepath = _filearchivebasepath + "/export/docs";
    private static readonly string _hashfilename = _filearchivepath + "/.db.hash.txt";
    private static readonly string _templateInvoice = _filearchivebasepath + "/Invoice.pdf";
    private static readonly string _templatePrescription = _filearchivebasepath + "/Prescription.pdf";
    private static string? _initialized = null;

    public FileStorageService(ILogger<FileStorageService> logger, IDatabaseService dbservice)
    {
        _logger = logger;
        _dbservice = dbservice;
    }

    public FileStream? GetFile(int id)
    {
        // TODO: Look up filename for the given id
        string filename = "Prescription.pdf";
        return GetFile(filename);
    }

    public FileStream? GetFile(string filename)
    {
        if (_initialized == null) Initialize();

        string filepath = Path.Combine(_filearchivepath, filename);
        _logger.LogDebug($"DBG GetFile(): filepath: {filepath}");

        if (File.Exists(filepath) || Directory.Exists(filepath))
        {
            _logger.LogDebug($"DBG GetFile(): File {filepath} exists: {File.Exists(filepath)} Dir exists: {Directory.Exists(filepath)}");
            return System.IO.File.OpenRead(filepath);  // Use System.IO to avoid naming confusion
        } else
        {
            _logger.LogDebug($"DBG GetFile(): File {filepath} does not exist!");
            return null;
        }
    }



    protected void Initialize()
    {
        _initialized = InitFileStorage(_dbservice);
    }

    public string? InitFileStorage(IDatabaseService dbservice)
    {
        string? dbhash = dbservice.GetDbHash();
        _logger.LogInformation($"DEBUG InitFileStorage(): DB hash: {dbhash}");
        if (dbhash == null) return null;
     
        
        UserDTO[]? users = dbservice.GetAllUsers();
        if (users == null) return null;


        // If hashvalue does not match, rebuild links
        if (!MatchesFolderHash(dbhash)) {
            // Remove existsing links
            DeleteFileArchive(_filearchivepath);

            // Create new links
            BuildFileArchive(_filearchivepath, dbhash, users);
        } 
        return dbhash;
        
    }

    private bool MatchesFolderHash(string dbhash)
    {
        _logger.LogInformation($"DEBUG MatchesFolderHash(): Loading: {_hashfilename}");

        try
        {
            string folderhash = File.ReadAllText(_hashfilename);
            _logger.LogInformation($"DEBUG MatchesFolderHash(): Folder hash: {folderhash}");

            return folderhash.Trim() == dbhash.Trim();
        }
        catch (IOException err)
        {
            _logger.LogWarning($"MatchesFolderHash(): Exception: {err.Message}");
            _logger.LogInformation($"DEBUG MatchesFolderHash(): The {_hashfilename} does not exist, returning 'false'");
            return false;
        }
    }

    private void BuildFileArchive(string path, string hashvalue, UserDTO[] users)
    {
        _logger.LogInformation("Rebuilding FolderArchive symlinks");
        foreach (UserDTO user in users)
        {
            // Create prescriptions
            if (user.Prescriptions != null)
            {
                foreach (PrescriptionDTO doc in user.Prescriptions)
                {
                    string filepath = doc.Filename;
                    string docpath = Path.Combine(_filearchivepath, filepath);
                    CreateSymLink(_templatePrescription, docpath);
                }
            }
            // Create invoices
            if (user.Invoices != null)
            {
                foreach (InvoiceDTO doc in user.Invoices)
                {
                    string filepath = doc.Filename;
                    string docpath = Path.Combine(_filearchivepath, filepath);
                    CreateSymLink(_templateInvoice, docpath);
                }
            }
        }

        File.WriteAllText(_hashfilename, hashvalue);
    }

    private void DeleteFileArchive(string path)
    {
        try
        {
            _logger.LogInformation($"DEBUG: DeleteFileArchive({path})");
            if (Directory.Exists(path))
            {
                _logger.LogInformation($"DEBUG: DeleteFileArchive(): Deleting {path}");
                Directory.Delete(path, true);
                _logger.LogInformation($"DEBUG: DeleteFileArchive(): Deleted {path}");
            }
        }
        catch (IOException err)
        {
            _logger.LogWarning("Error deleting FolderArchive: " + err.Message);
        }
    }

    private void CreateSymLink(string targetpath, string docpath)
    {
        string path = Path.GetDirectoryName(docpath);
        string filename =  Path.GetFileName(docpath);
        string target = Path.GetRelativePath(path, targetpath);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        _logger.LogInformation($"DEBUG: DeleteFileArchive(): Creating {path}");
        File.CreateSymbolicLink(Path.Combine(path, filename), target);
    }
}
