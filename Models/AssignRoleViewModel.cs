﻿namespace Blanca_eManagement.Models
{
    public class AssignRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
