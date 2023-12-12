﻿namespace Pronia.ViewModels
{
    public class LoginVm
    {
        [Required]
        public string UsernameOrEmail { get; set; }
        [Required]
        [DataType (DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
