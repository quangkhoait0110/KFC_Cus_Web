namespace Project_KFC_WEB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("food")]
    public partial class food
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public food()
        {
            carts = new HashSet<cart>();
        }

        [DisplayName("Mã món ăn")]
        public int id { get; set; }

        [DisplayName("Loại món ăn")]
        public int? idCategory { get; set; }

        [DisplayName("Tên món ăn")]
        public string name { get; set; }

        [DisplayName("Ảnh")]
        public string image { get; set; }

        [DisplayName("Giá")]
        public double? price { get; set; }

        [DisplayName("Giảm giá(%)")]
        public double? discount { get; set; }

        [DisplayName("Mô tả")]
        public string description { get; set; }

        [DisplayName("Ngày giảm giá")]
        [Column(TypeName = "date")]
        public DateTime? timeSellStart { get; set; }

        [DisplayName("Ngày kết thúc")]
        [Column(TypeName = "date")]
        public DateTime? timeSellEnd { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cart> carts { get; set; }

        public virtual foodCategory foodCategory { get; set; }
    }
}
