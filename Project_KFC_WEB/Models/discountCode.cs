namespace Project_KFC_WEB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("discountCode")]
    public partial class discountCode
    {
        [DisplayName("Mã")]
        public int id { get; set; }

        [DisplayName("Mã giảm giá")]
        [StringLength(50)]
        public string code { get; set; }

        [DisplayName("Lượng giảm giá(%)")]
        public double? discount { get; set; }
    }
}
