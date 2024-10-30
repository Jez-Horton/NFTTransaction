using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlluviumTest.Models
{
    public class NFT
    {
        [Key]
        public string TokenId { get; set; } = null!;
        public string OwnerAddress { get; set; } = null!;
    }
}
