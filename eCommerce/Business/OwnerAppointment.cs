using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class OwnerAppointment
    {
        [Key]
        public string OwnerId { get; set; }
        public string Ownername { get; set; }
        public User User { get; set; }
        private List<StorePermission> _permissions;


        //for ef
        public OwnerAppointment()
        {
            // we are able to simply re-assign all permissions to every owner loaded from DB,
            // because, until further notice, by definition all owners have every permission.
            this._permissions = new List<StorePermission>();

            foreach (var permission in Enum.GetValues(typeof(StorePermission)))
            {
                _permissions.Add((StorePermission)permission);
            }
        }

        public OwnerAppointment(User user, string storename)
        {
            this.User = user;
            this.OwnerId = user.Username + "_" + storename;
            this.Ownername = user.Username;
            this._permissions = new List<StorePermission>();

            foreach (var permission in Enum.GetValues(typeof(StorePermission)))
            {
                _permissions.Add((StorePermission)permission);
            }
        }

        public Result HasPermission(StorePermission permission)
        {
            if(_permissions.Contains(permission))
                return Result.Ok();
            return Result.Fail("Owner does not have the required permission");
        }
    }
}