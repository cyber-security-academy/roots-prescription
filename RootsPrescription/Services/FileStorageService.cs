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

        if (File.Exists(filepath) || Directory.Exists(filepath))
        {
            return System.IO.File.OpenRead(filepath);  // Use System.IO to avoid naming confusion
        } else
        {
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
        _logger.LogDebug("InitFileStorage(): DB hash: {Hash}", dbhash);
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
        _logger.LogDebug("MatchesFolderHash(): Loading: {HashFilename}", _hashfilename);
        if (!File.Exists(_hashfilename)) return false; 

        try
        {
            string folderhash = File.ReadAllText(_hashfilename);
            _logger.LogDebug("MatchesFolderHash(): Folder hash: {Hash}", folderhash);

            return folderhash.Trim() == dbhash.Trim();
        }
        catch (IOException err)
        {            
            _logger.LogError("MatchesFolderHash(): Exception: {ErrMessage}", err.Message);
            _logger.LogError("MatchesFolderHash(): The {HashFilename} does not exist, returning 'false'", _hashfilename);
            return false;
        }
    }

    private void BuildFileArchive(string path, string hashvalue, UserDTO[] users)
    {
        _logger.LogInformation("Rebuilding symlinks in folder archive");
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
        _logger.LogDebug("BuildFileArchive(): Creating: {HashFilename}, with hashvalue: {HashValue}", _hashfilename, hashvalue);
        File.WriteAllText(_hashfilename, hashvalue);
        _logger.LogDebug("BuildFileArchive(): Verify: Reading {HashFilename}: {HashValue}", _hashfilename, File.ReadAllText(_hashfilename));
    }

    private void DeleteFileArchive(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                _logger.LogDebug("DEBUG: DeleteFileArchive(): Deleting {Path}", path);
                Directory.Delete(path, true);
            }
        }
        catch (IOException err)
        {
            _logger.LogError("Error deleting FolderArchive: {ErrMessage}", err.Message);
        }
    }

    private void CreateSymLink(string targetpath, string docpath)
    {
        string path = Path.GetDirectoryName(docpath);
        string filename =  Path.GetFileName(docpath);
        string target = Path.GetRelativePath(path, targetpath);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        File.CreateSymbolicLink(Path.Combine(path, filename), target);
    }
}
