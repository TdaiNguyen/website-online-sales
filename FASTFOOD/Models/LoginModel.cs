using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FASTFOOD.Models
{
    public class LoginModel
    {
         [Required(ErrorMessage = "Không được để trống!")]
         [MaxLength(50, ErrorMessage = "Tên tài khoản tối đa 50 ký tự!")]
         public string Email { set; get; }
         [Required(ErrorMessage = "Không được để trống!")]
         [MaxLength(50, ErrorMessage = "Mật khẩu tối đa 50 ký tự!")]
         public string Password { set; get; }
         public bool Remenberme { get; set; }
        }
    }

