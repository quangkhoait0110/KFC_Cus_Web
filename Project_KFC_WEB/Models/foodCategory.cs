namespace Project_KFC_WEB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("foodCategory")]
    public partial class foodCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public foodCategory()
        {
            foods = new HashSet<food>();
        }

        [DisplayName("Mã loại")]
        public int id { get; set; }

        [DisplayName("Tên loại món ăn")]
        public string name { get; set; }

        [DisplayName("Ảnh")]
        public string image { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<food> foods { get; set; }
    }
}
