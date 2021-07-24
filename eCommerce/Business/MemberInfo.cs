using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class MemberInfo : ICloneable<MemberInfo>
    {
        public string Id { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }

        public MemberInfo(string username, string email, string name, DateTime birthday, string address)
        {
            // TODO: may change id to real id
            Id = username;
            Username = username;
            Email = email;
            Name = name;
            Birthday = birthday;
            Address = address;
        }

        public Result IsBasicDataFull()
        {
            IList<string> emptyOrNullFields = new List<string>();
            if (string.IsNullOrEmpty(Username))
            {
                emptyOrNullFields.Add("Username");
            }

            if (string.IsNullOrEmpty(Email))
            {
                emptyOrNullFields.Add("Email");
            }

            if (string.IsNullOrEmpty(Name))
            {
                emptyOrNullFields.Add("Name");
            }

            if (string.IsNullOrEmpty(Address))
            {
                emptyOrNullFields.Add("Address");
            }

            if (emptyOrNullFields.Count > 0)
            {
                return Result.Fail($"This fields are empty or null: {emptyOrNullFields.ToString()}");
            }

            return Result.Ok();
        }

        public MemberInfo Clone()
        {
            return (MemberInfo) this.MemberwiseClone();
        }
    }
}