using Microsoft.AspNetCore.Mvc;
using RootsPrescription.Models;
using System.Security.Cryptography;
using System.Text.Json;

/// <summary>
/// THIS FILE SIMULATES THE INTERFACE TO A PROPER DATABASE
/// THIS IS NOT PART OF THE CYBER SECURITY ACADEMY COURSE
///
/// DO NOT WASTE TIME ON THIS FILE
/// </summary>


namespace RootsPrescription.Database;
public class DatabaseService : IDatabaseService
{
    private readonly ILogger<DatabaseService> _logger;
    private static string? _dataHash = null;
    private static UserDTO[] _data = null;
    private static Dictionary<int,UserDTO> _userIds;
    private static Dictionary<string,UserDTO> _userNames;
    private static Dictionary<string,UserDTO> _userNatIds;
    private static Dictionary<int,InvoiceDTO> _invoices;
    private static Dictionary<int,PrescriptionDTO> _prescriptions;


    public DatabaseService(ILogger<DatabaseService> logger)
    {
        _logger = logger;

        LoadJsonToLookupTables(@"Data/user-info.json");
    }

    private void LoadJsonToLookupTables(string filename)
    {
        // TODO: Fix this to be a working singleton for the whole application
        if (_data == null)
        {
            _logger.LogInformation("Loading JSON database '{Filename}'", filename);
            _userIds = new();
            _userNames = new();
            _userNatIds = new();
            _prescriptions = new();
            _invoices = new();


            using (FileStream file = File.OpenRead(filename))
            {
                long pos = file.Position;  //  Store position
                _data = JsonSerializer.Deserialize<UserDTO[]>(file);

                file.Position = pos; // Rewind and restore posiion of filestream
                MD5 md5 = MD5.Create();
                _dataHash = Convert.ToHexString(md5.ComputeHash(file));
            }

            if (_data == null) throw new FileLoadException($"Could not load JSON data file '{filename}'");

            // Loop Users
            foreach (UserDTO user in _data)
            {
                // Add user lookups
                _userIds.Add(user.Id, user);
                _userNames.Add(user.UserName.ToLower(), user);
                _userNatIds.Add(user.NationalIdNumber, user);

                // Extract prescriptions
                if (user != null && user.Prescriptions != null)
                {
                    foreach (PrescriptionDTO prescription in user.Prescriptions)
                    {
                        prescription.OwnerId = user.Id;
                        prescription.Filename = user.Id + "/Resept-" + prescription.DocNo + ".pdf";
                        _prescriptions.Add(prescription.Id, prescription);
                    }
                }
                if (user != null && user.Invoices != null)
                {
                    foreach (InvoiceDTO invoice in user.Invoices)
                    {
                        invoice.OwnerId = user.Id;
                        invoice.Filename = user.Id + "/Faktura-" + invoice.InvoiceNo + ".pdf";
                        _invoices.Add(invoice.Id, invoice);
                    }

                }
            }
            _logger.LogInformation("Loaded {Count} users from JSON database", _data.Length);
        }
    }

    public UserDTO[]? GetAllUsers()
    {
        return _data;
    }

    public UserDTO? GetUserById(int id, bool returnExtendedObject=false)
    {
        UserDTO? user = _userIds.ContainsKey(id) ? _userIds[id] : null;
        if (returnExtendedObject || user == null)
        {
            return user;
        }
        else
        {
            return user.Prune();
        }
    }

    public UserDTO? GetUserByUsername(string username)
    {
        username = username.ToLower().Trim();
        UserDTO? user = _userNames.ContainsKey(username) ? _userNames[username] : null;
        return user == null ? null : user.Prune();
    }

    public UserDTO? GetUserByNationalId(string nationalId)
    {
        UserDTO? user = _userNatIds.ContainsKey(nationalId) ? _userNatIds[nationalId] : null;
        return user == null ? null : user.Prune();
    }

    public PrescriptionDTO? GetPrescription(int id)
    {
        if (_prescriptions.ContainsKey(id))
        {
            return _prescriptions[id];
        } else
        {
            return null;
        }
    }

    public InvoiceDTO? GetInvoice(int id)
    {
        if (_invoices.ContainsKey(id))
        {
            return _invoices[id];
        } else
        {
            return null;
        }
    }

    public InvoiceDTO? GetInvoice(string filename)
    {
        return _invoices.FirstOrDefault(x => x.Value.Filename == filename).Value;
    }

    public PrescriptionDTO[]? GetUserPrescriptions(int userid)
    {
        UserDTO user = GetUserById(userid, true);
        if (user == null) return null;
        return user.Prescriptions;
    }

    public InvoiceDTO[]? GetUserInvoices(int userid)
    {
        UserDTO user = GetUserById(userid, true);
        if (user == null) return null;
        return user.Invoices;
    }

    public string? GetDbHash()
    {
        return _dataHash;
    }
}
