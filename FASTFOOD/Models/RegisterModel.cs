using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FASTFOOD.Models
{
    public class RegisterModel
    {
        public string FirstName { set; get; }
        public string LastName { set; get; }
        [Required(ErrorMessage = "Không được để trống!")]
        [MaxLength(50, ErrorMessage = "Tên tài khoản tối đa 50 ký tự!")]
        public string Email { set; get; }
        [Required(ErrorMessage = "Không được để trống!")]
        [MaxLength(50, ErrorMessage = "Mật khẩu tối đa 50 ký tự!")]
        public string Password { set; get; }
        [Required(ErrorMessage = "Không được để trống!")]
        [MaxLength(50, ErrorMessage = "Mật khẩu tối đa 50 ký tự!")]

        public string DiaChi { set; get; }
        public string ConfirmPassword { get; set; }
    }
}