using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using eCommerce.Business.Repositories;
using eCommerce.Common;
using eCommerce.DataLayer;

namespace eCommerce.Business
{
    public class ManagerAppointment
    {
        [Key]
        public string ManagerId { get; set; }
        // public string Managername { get; set; }
        // public string ManagedStorename { get; set; }
        public User User { get; set; }
        //private ConcurrentDictionary<StorePermission, bool> _permissions;
        private List<StorePermission> _permissions;
        private static readonly  IList<StorePermission> BaseManagerPermissions = ImmutableList.Create(new StorePermission[] 
            {StorePermission.ViewStaffPermission});

        public ManagerAppointment(User user, string storename)
        {
            this.ManagerId = user.Username+"_"+storename;
            this.User = user;
            // this.Managername = user.Username;
            // this.ManagedStorename = storename;
            this._permissions = new List<StorePermission>();
            foreach (StorePermission permission in BaseManagerPermissions)
            {
                _permissions.Add(permission);
            }
        }
        
        public Result AddPermissions(StorePermission permission)
        {
            _permissions.Add(permission);
            return Result.Ok();
        }
        
        public Result RemovePermission(StorePermission permission)
        {
            bool btrue;
            btrue=_permissions.Remove(permission);
            if (btrue)
            {
                return Result.Ok();
            }
            else
            {
                return Result.Fail("Manager does not have the given permission");
            }
        }

        public Result HasPermission(StorePermission permission)
        {
            if(_permissions.Contains(permission))
                return Result.Ok();
            return Result.Fail("Manager does not have the required permission");
        }

        public Result UpdatePermissions(IList<StorePermission> permissions)
        {
            bool res = false;
            var newPermissions = new List<StorePermission>();
            foreach (var permission in permissions)
            {
                newPermissions.Add(permission);
            }

            foreach (var permission in BaseManagerPermissions)
            {
                if (!newPermissions.Contains(permission))
                {
                    newPermissions.Add(permission);
                }
            }
            this._permissions = newPermissions;
            return Result.Ok();
        }

        public List<StorePermission> GetAllPermissions()
        {
            return _permissions.ToList();
        }


        #region EF
        public virtual string permissionsString { get; set;}
        public ManagerAppointment()
        {
            this._permissions = new List<StorePermission>();
            this.permissionsString = "";
        }

         public void syncFromDict()
        {
            permissionsString = string.Join(";", _permissions.Select(p => (int)p));
            Console.WriteLine(permissionsString);
        }
         
         
        public void syncToDict()
        {
            string[] list = permissionsString.Split(";");
            _permissions = new List<StorePermission>();
            foreach (string p in list)
            {
                _permissions.Add((StorePermission)int.Parse(p));
            }
        }

       

        #endregion EF
        
    }
}