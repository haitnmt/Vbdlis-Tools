using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Haihv.Vbdlis.Tools.Desktop.Entities
{
    [Table("DvhcCapTinh")]
    public class DvhcCapTinh
    {
        /// <summary>
        /// Mã Đơn vị hành chính cấp tỉnh
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Tên Đơn vị hành chính cấp tỉnh
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        /// <summary>
        /// Ngày cập nhật bản ghi
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
