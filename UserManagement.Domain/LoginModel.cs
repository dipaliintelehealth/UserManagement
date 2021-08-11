﻿namespace UserManagement.Domain
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReferenceId { get; set; }
        public bool IsActive { get; set; }
        public string SourceId { get; set; }
    }
    public class LoginModelForCsv
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReferenceId { get; set; }
        public bool IsActive { get; set; }
        public string SourceId { get; set; }
    }
}
