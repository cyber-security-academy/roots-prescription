﻿using Microsoft.AspNetCore.Mvc;
using RootsPrescription.Models;

namespace RootsPrescription.Database;
public interface IDatabaseService
{
    UserDTO[]? GetAllUsers();
    UserDTO? GetUserById(int userid, bool extendedUser = false);
    UserDTO? GetUserByUsername(string username);
    UserDTO? GetUserByNationalId(string nationalId);
    InvoiceDTO? GetInvoice(int id);
    InvoiceDTO? GetInvoice(string filename);
    PrescriptionDTO? GetPrescription(int id);
    InvoiceDTO[] GetUserInvoices(int userid);
    PrescriptionDTO[] GetUserPrescriptions(int userid);

    string? GetDbHash();
}
