using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.FEntities
{
    public class holiday_mast_table
    {
        public DateTime? lchg_time { get; set; } // Maps to 'timestamp without time zone'
        public DateTime? lchg_Day { get; set; } // Maps to 'timestamp without time zone'
        public string mmyyyy { get; set; }      // Maps to 'character varying'
        public string? hldy_str { get; set; }    // Matches `hldy_str` in SQL
        public int? ts_cnt { get; set; }        // Matches `ts_cnt` in SQL
    }
}
