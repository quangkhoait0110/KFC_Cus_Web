namespace Project_KFC_WEB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("account")]
    public partial class account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public account()
        {
            carts = new HashSet<cart>();
        }

        [Key]
        [StringLength(50)]
        [DisplayName("Tên đăng nhập")]
        public string userName { get; set; }

        [DisplayName("Mật khẩu")]
        [StringLength(50)]
        public string passWord { get; set; }

        [DisplayName("Tên người dùng")]
        public string name { get; set; }

        [DisplayName("Địa chỉ")]
        public string address { get; set; }

        [DisplayName("Số điện thoại")]
        [StringLength(10)]
        public string phone { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cart> carts { get; set; }
    }
}
