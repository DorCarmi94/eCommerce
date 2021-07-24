using System;
using System.Collections.Generic;
using System.IO;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace eCommerce
{
    public class BasicActionJsonFormat {
        public string Action { get; set; }
        public JObject Data { get; set; }
    }
    
    public class CreateUserData {
        public MemberInfo MemberInfo { get; set; }
        public string Password { get;  set; }
    }
    
    public class MemberAction {
        public string Username { get; set; }
        public string Password { get; set; }
        public BasicActionJsonFormat[] Actions { get; set; }
    }
    
    public class AppointManager {
        public string Manager { get; set; }
        public string Store { get; set; }
        public string[] Permissions { get; set; }
    }

    public class InitSystemWithData
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private IAuthService _authService;
        private IUserService _userService;
        private INStoreService _inStoreService;
        
        private string _guestToken;
        private string _workingToken;

        public InitSystemWithData(IAuthService authService, IUserService userService, INStoreService inStoreService)
        {
            _authService = authService;
            _userService = userService;
            _inStoreService = inStoreService;
        }

        private void InitServices()
        {
            _guestToken = _authService.Connect();
        }

        private void Setup()
        {
            _guestToken = _authService.Connect();
        }
        
        private void CleanUp()
        { 
            _authService.Disconnect(_guestToken);
            //_authService.Disconnect(_authService.Logout(_workingToken).Value);
        }

        public bool Init(string initFile)
        {
            InitServices();
            return InitData(initFile);
        }

        private bool InitData(string initFile)
        {
            if (!File.Exists(initFile))
            {
                _logger.Warn($"Init file {initFile} doesn't exisits");
                return false;
            }
            
            int at = 0;
            BasicActionJsonFormat[] initDataJsons = JsonConvert.DeserializeObject<BasicActionJsonFormat[]>(File.ReadAllText(initFile));
            if (initDataJsons == null)
            {
                _logger.Warn($"Init file {initFile} empty or not in the required format");
                return false;
            }
                
            Setup();
            foreach (var initData in initDataJsons)
            {
                switch (initData.Action)
                {
                    case "CreateUser":
                    {
                        CreateUserData userData = initData.Data.ToObject<CreateUserData>();
                        Result registerRes = _authService.Register(_guestToken, userData.MemberInfo, userData.Password).Result;
                        if (registerRes.IsFailure)
                        {
                            LogError($"Error when register user {userData.MemberInfo.Username}",
                                registerRes.Error, at, initFile);
                        }
                        break;
                    }
                    case "MemberAction":
                    {
                        MemberAction memberAction = initData.Data.ToObject<MemberAction>();
                        HandleMemberActions(memberAction, at, initFile);
                        break;
                    }
                    default:
                        ThrowInvalidException($"Invalid init action {initData.Action}",
                            "Invalid action", at, initFile);
                        break;
                }

                at++;
            }

            CleanUp();
            return true;
        }

        private void HandleMemberActions(MemberAction memberAction, int at, string initFile)
        {
            _workingToken = _authService.Connect();
            Result<string> loginRes = _authService.Login(_workingToken, memberAction.Username,
                memberAction.Password, ServiceUserRole.Member).Result;
            if (loginRes.IsFailure)
            {
                LogError($"User {memberAction.Username} wasn't able to login",
                    loginRes.Error, at, initFile);
                return;
            }

            _workingToken = loginRes.Value;
            
            foreach (var basicAction in memberAction.Actions)
            {
                switch (basicAction.Action)
                {
                    case "OpenStore":
                    {
                        string storeName = basicAction.Data["StoreName"].ToString();
                        Result openStoreRes = _inStoreService.OpenStore(_workingToken, storeName);
                        if (openStoreRes.IsFailure)
                        {
                            LogError($"Error when opening store {storeName} for {memberAction.Username}",
                                openStoreRes.Error, at, initFile);
                        }
                        break;
                    }
                    case "AddItem":
                    {
                        SItem item = basicAction.Data.ToObject<SItem>();
                        Result addItemRes = _inStoreService.AddNewItemToStore(_workingToken, item);
                        if (addItemRes.IsFailure)
                        {
                            LogError($"Error when adding item {item.ItemName} to store {item.StoreName}",
                                addItemRes.Error, at, initFile);
                        }
                        break;
                    }
                    case "AppointManager":
                    {
                        AppointManager appointManager = basicAction.Data.ToObject<AppointManager>();
                        Result appointManagerRes = _userService.AppointManager(_workingToken, appointManager.Store, appointManager.Manager);
                        if (appointManagerRes.IsFailure)
                        {
                            LogError($"Error when appointing {appointManager.Manager} to store {appointManager.Store}",
                                appointManagerRes.Error, at, initFile);
                            break;
                        }

                        if (appointManager.Permissions != null && appointManager.Permissions.Length > 0)
                        {
                            List<StorePermission> managerPermissions = new List<StorePermission>();
                            foreach (var permission in appointManager.Permissions)
                            {
                                try
                                {
                                    StorePermission storePermission = Enum.Parse<StorePermission>(permission, true);
                                    managerPermissions.Add(storePermission);
                                }
                                catch (Exception e)
                                {
                                    LogError($"Invalid permission {permission} when appointing {appointManager.Permissions} to store {appointManager.Store}",
                                        appointManagerRes.Error, at, initFile);
                                }
                            }

                            Result updateManagerPermission = _userService.UpdateManagerPermission(_workingToken, appointManager.Store,
                                appointManager.Manager, managerPermissions);
                            if (updateManagerPermission.IsFailure)
                            {
                                LogError($"Error when updating {appointManager.Manager} manager permission on store {appointManager.Store}",
                                    appointManagerRes.Error, at, initFile);
                            }
                        }
                        break;
                    }
                    default:
                        ThrowInvalidException($"Invalid init action at index {at} of MemberAction",
                            "Invalid action", at, initFile);
                        break;
                }
            }
            
            _authService.Disconnect(_authService.Logout(_workingToken).Value);
        }

        private void LogError(string errorMessage, string resMessage, int at, string initFile)
        {
            CleanUp();
            string message = $"\nSystem Init:\n{errorMessage}\n" +
                             $"Error message: {resMessage}\n" +
                             $"In index {at}, file {initFile}";
            _logger.Error(message);
        }
        
        private void ThrowInvalidException(string errorMessage, string resMessage, int at, string initFile)
        {
            CleanUp();
            string message = $"\nSystem Init:\n{errorMessage}\n" +
                             $"Error message: {resMessage}\n" +
                             $"In index {at}, file {initFile}";
            _logger.Error(message);
            throw new InvalidDataException(message);
        }
    }
}