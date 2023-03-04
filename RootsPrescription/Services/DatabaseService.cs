using Microsoft.AspNetCore.Mvc;
using RootsPrescription.Models;
using System.Text.Json;

/// <summary>
/// THIS FILE SIMULATES THE INTERFACE TO A PROPER DATABASE
/// </summary>


namespace RootsPrescription.Database;
public class DatabaseService : IDatabaseService
{
    private readonly ILogger<DatabaseService> _logger;
    private static UserDTO[] _data;
    private static Dictionary<int,UserDTO> _userIds;
    private static Dictionary<string,UserDTO> _userNames;
    private static Dictionary<string,UserDTO> _userNatIds;
    private static Dictionary<int,InvoiceDTO> _invoices;
    private static Dictionary<int,PrescriptionDTO> _prescriptions;


    public DatabaseService(ILogger<DatabaseService> logger)
    {
        _logger = logger;
        _data = null;
        _prescriptions = new();
        _invoices = new();
        _userIds = new();
        _userNames = new();
        _userNatIds = new();

        LoadJsonToLookupTables(@"Data/user-info.json");
    }
    private void LoadJsonToLookupTables(string filename)
    {
        // TODO: Fix this to be a working singleton for the whole application
        if (_data == null)
        {
            _logger.LogInformation($"Loading JSON database '{filename}'");
            using (FileStream file = File.OpenRead(filename))
            {
                _data = JsonSerializer.Deserialize<UserDTO[]>(file);
            }

            if (_data == null) throw new FileLoadException($"Could not load JSON data file '{filename}'");

            // Loop Users
            foreach (UserDTO user in _data)
            {
                // Add user lookups
                _userIds.Add(user.Id, user);
                _userNames.Add(user.UserName.ToLower(), user);
                _userNatIds.Add(user.NationalIdNumber, user);

                // Extract perscriptions
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
            _logger.LogInformation($"Loaded {_data.Length} users from JSON database");
        }
    }



    public UserDTO? GetUserById(int id)
    {
        return _userIds.ContainsKey(id) ? _userIds[id] : null;
    }
    public UserDTO? GetUserByUsername(string username)
    {
        username = username.ToLower().Trim();
        return _userNames.ContainsKey(username) ? _userNames[username] : null;
    }

    public UserDTO? GetUserByNationalId(string nationalId)
    {
        return _userNatIds.ContainsKey(nationalId) ? _userNatIds[nationalId] : null;
    }

    public PrescriptionDTO GetPrescription(int id)
    {
        if (_prescriptions.ContainsKey(id))
        {
            return _prescriptions[id];
        } else
        {
            return null;
        }
    }
    public InvoiceDTO GetInvoice(int id)
    {
        if (_invoices.ContainsKey(id))
        {
            return _invoices[id];
        } else
        {
            return null;
        }
    }
    public PrescriptionDTO[] GetUserPrescriptions(int userid)
    {
        UserDTO user = GetUserById(userid);
        if (user == null) return null;
        return user.Prescriptions;
    }
    public InvoiceDTO[] GetUserInvoices(int uid)
    {
        UserDTO user = GetUserById(uid);
        if (user == null) return null;
        return user.Invoices;
    }
}
