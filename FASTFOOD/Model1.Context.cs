﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FASTFOOD
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class fastfoodEntities : DbContext
    {
        public fastfoodEntities()
            : base("name=fastfoodEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DATMONAN> DATMONANs { get; set; }
        public virtual DbSet<HOADON> HOADONs { get; set; }
        public virtual DbSet<LOAIMONAN> LOAIMONANs { get; set; }
        public virtual DbSet<MONAN> MONANs { get; set; }
        public virtual DbSet<TAIKHOAN> TAIKHOANs { get; set; }
    }
}