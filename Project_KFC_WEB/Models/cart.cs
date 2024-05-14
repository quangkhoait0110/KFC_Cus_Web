namespace Project_KFC_WEB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cart")]
    public partial class cart
    {
        [DisplayName("Mã đơn hàng")]
        public int id { get; set; }

        [DisplayName("Món ăn")]
        public int? idFood { get; set; }

        [DisplayName("Tên người dùng")]
        [StringLength(50)]
        public string userName { get; set; }

        [DisplayName("Số lượng")]
        public int? quantity { get; set; }

        [DisplayName("Tổng hóa đơn")]
        public double? totalPrice
        {
            get
            {
                if (quantity != null && quantity > 0 && food != null)
                {
                    if (food.discount != null && food.discount > 0)
                    {
                        return quantity * (food.price * ((100 - food.discount) / 100));
                    }
                    else
                    {
                        return quantity * food.price;
                    }
                }
                else return 0;
            }
            set
            {

            }
        }

        public virtual account account { get; set; }

        public virtual food food { get; set; }
    }
}
