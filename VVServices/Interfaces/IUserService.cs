using VVData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces;

public interface IUserService
{
    public Task<bool> UserExists(string email);
	public UserInfoViewModel FindUser(string uniqueToken);
	public Task CreateUser(UserInfoViewModel info);
    public void UpdateUser(UserInfoViewModel info);
    public List<String> GetAllergies(string userSID);
}
