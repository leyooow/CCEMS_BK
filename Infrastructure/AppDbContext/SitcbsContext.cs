using Microsoft.EntityFrameworkCore;
using Domain.FEntities;

public partial class SitcbsContext : DbContext
{
    public SitcbsContext(DbContextOptions<SitcbsContext> options)
        : base(options)
    {
    }

    
    public virtual DbSet<BranchCodeTable> BranchCodeTables { get; set; }
    public virtual DbSet<holiday_mast_table> holiday_mast_tables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureBranchCodeTable(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
    }

    private static void ConfigureBranchCodeTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BranchCodeTable>(entity =>
        {
            // Map the table name and schema
            entity.ToTable("branch_code_table", "tbaadm");

            // Define the composite primary key
            entity.HasKey(e => new { e.BrCode });

            // Configure individual columns (examples for key fields)
            entity.Property(e => e.BrCode)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("br_code");


            // Additional column configurations (example)
            entity.Property(e => e.BrName)
                .HasMaxLength(100)
                .HasColumnName("br_name");

       


            // Add more configurations for other columns as needed
        });
        modelBuilder.Entity<holiday_mast_table>(entity =>
        {
            entity.ToTable("holiday_mast_table", "tbaadm");

            modelBuilder.Entity<holiday_mast_table>().HasNoKey();
            // Map properties if needed, e.g.:
            //entity.Property(e => e.CalB2kId)
            //    .HasMaxLength(12)
            //    .IsRequired(); // Adjust constraints as per your schema.
        });

        //modelBuilder.Entity<BranchCodeTable>(entity =>
        //{
        //    // Map the table name and schema
        //    entity.ToTable("branch_code_table", "tbaadm");

        //    // Define the composite primary key
        //    entity.HasKey(e => new { e.BrCode, e.BankCode, e.BankId });

        //    // Configure individual columns (examples for key fields)
        //    entity.Property(e => e.BrCode)
        //        .IsRequired()
        //        .HasMaxLength(6)
        //        .HasColumnName("br_code");

        //    entity.Property(e => e.BankCode)
        //        .IsRequired()
        //        .HasMaxLength(6)
        //        .HasColumnName("bank_code");

        //    entity.Property(e => e.BankId)
        //        .IsRequired()
        //        .HasMaxLength(8)
        //        .HasColumnName("bank_id");

        //    // Additional column configurations
        //    entity.Property(e => e.BrName)
        //        .HasMaxLength(30)
        //        .HasColumnName("br_name");

        //    entity.Property(e => e.BrShortName)
        //        .HasMaxLength(10)
        //        .HasColumnName("br_short_name");

        //    entity.Property(e => e.BrAddr1)
        //        .HasMaxLength(45)
        //        .HasColumnName("br_addr_1");

        //    entity.Property(e => e.BrAddr2)
        //        .HasMaxLength(45)
        //        .HasColumnName("br_addr_2");

        //    entity.Property(e => e.BrCityCode)
        //        .HasMaxLength(5)
        //        .HasColumnName("br_city_code");

        //    entity.Property(e => e.BrStateCode)
        //        .HasMaxLength(5)
        //        .HasColumnName("br_state_code");

        //    entity.Property(e => e.BrPinCode)
        //        .HasMaxLength(10)
        //        .HasColumnName("br_pin_code");

        //    entity.Property(e => e.BrWklyOff)
        //        .HasColumnType("numeric(1,0)")
        //        .HasColumnName("br_wkly_off");

        //    entity.Property(e => e.LocalClgFlg)
        //        .HasMaxLength(1)
        //        .HasColumnName("local_clg_flg");

        //    entity.Property(e => e.SplCollCentreFlg)
        //        .HasMaxLength(1)
        //        .HasColumnName("spl_coll_centre_flg");

        //    entity.Property(e => e.DdIssBrCode)
        //        .HasMaxLength(6)
        //        .HasColumnName("dd_iss_br_code");

        //    entity.Property(e => e.DdIssBrName)
        //        .HasMaxLength(30)
        //        .HasColumnName("dd_iss_br_name");

        //    entity.Property(e => e.CollAgentBrCode)
        //        .HasMaxLength(6)
        //        .HasColumnName("coll_agent_br_code");

        //    entity.Property(e => e.InwardTtFlg)
        //        .HasMaxLength(1)
        //        .HasColumnName("inward_tt_flg");

        //    entity.Property(e => e.OutwardTtFlg)
        //        .HasMaxLength(1)
        //        .HasColumnName("outward_tt_flg");

        //    entity.Property(e => e.LchgUserId)
        //        .HasMaxLength(15)
        //        .HasColumnName("lchg_user_id");

        //    entity.Property(e => e.LchgTime)
        //        .HasColumnType("timestamp without time zone")
        //        .HasColumnName("lchg_time");

        //    entity.Property(e => e.RcreUserId)
        //        .HasMaxLength(15)
        //        .HasColumnName("rcre_user_id");

        //    entity.Property(e => e.RcreTime)
        //        .HasColumnType("timestamp without time zone")
        //        .HasColumnName("rcre_time");

        //    entity.Property(e => e.RoutingBrCode)
        //        .HasMaxLength(6)
        //        .HasColumnName("routing_br_code");

        //    entity.Property(e => e.BcbCode)
        //        .HasMaxLength(12)
        //        .HasColumnName("bcb_code");

        //    entity.Property(e => e.CollAgentBankCode)
        //        .HasMaxLength(6)
        //        .HasColumnName("coll_agent_bank_code");

        //    entity.Property(e => e.BrAddr3)
        //        .HasMaxLength(45)
        //        .HasColumnName("br_addr_3");

        //    entity.Property(e => e.CntryCode)
        //        .HasMaxLength(5)
        //        .HasColumnName("cntry_code");

        //    entity.Property(e => e.TlxNum)
        //        .HasMaxLength(10)
        //        .HasColumnName("tlx_num");

        //    entity.Property(e => e.PhoneNum)
        //        .HasMaxLength(20)
        //        .HasColumnName("phone_num");

        //    entity.Property(e => e.FaxNum)
        //        .HasMaxLength(25)
        //        .HasColumnName("fax_num");

        //    entity.Property(e => e.BankBrRmks)
        //        .HasMaxLength(30)
        //        .HasColumnName("bank_br_rmks");

        //    entity.Property(e => e.OurOfficeCode)
        //        .HasMaxLength(6)
        //        .HasColumnName("our_office_code");

        //    entity.Property(e => e.OurOfficeBankCode)
        //        .HasMaxLength(6)
        //        .HasColumnName("our_office_bank_code");

        //    entity.Property(e => e.FrnBrFlg)
        //        .HasMaxLength(1)
        //        .HasColumnName("frn_br_flg");

        //    entity.Property(e => e.RoutingBankCode)
        //        .HasMaxLength(6)
        //        .HasColumnName("routing_bank_code");

        //    entity.Property(e => e.FcIbtAlwdFlg)
        //        .HasMaxLength(1)
        //        .HasColumnName("fc_ibt_alwd_flg");

        //    entity.Property(e => e.DaysForValueDate)
        //        .HasColumnType("numeric(3,0)")
        //        .HasColumnName("days_for_value_date");

        //    entity.Property(e => e.TradeFinanceBranch)
        //        .HasMaxLength(1)
        //        .HasColumnName("trade_finance_branch");

        //    entity.Property(e => e.TsCnt)
        //        .HasColumnType("numeric(5,0)")
        //        .HasColumnName("ts_cnt");

        //    entity.Property(e => e.DcAlias)
        //        .HasMaxLength(2)
        //        .HasColumnName("dc_alias");

        //    entity.Property(e => e.ClgRefCode)
        //        .HasMaxLength(5)
        //        .HasColumnName("clg_ref_code");

        //    entity.Property(e => e.FreeCode1)
        //        .HasMaxLength(5)
        //        .HasColumnName("free_code_1");

        //    entity.Property(e => e.FreeCode2)
        //        .HasMaxLength(5)
        //        .HasColumnName("free_code_2");

        //    entity.Property(e => e.FreeCode3)
        //        .HasMaxLength(5)
        //        .HasColumnName("free_code_3");

        //    entity.Property(e => e.BrFreeCode1)
        //        .HasMaxLength(5)
        //        .HasColumnName("br_free_code_1");

        //    entity.Property(e => e.BrFreeCode2)
        //        .HasMaxLength(5)
        //        .HasColumnName("br_free_code_2");

        //    entity.Property(e => e.BrFreeCode3)
        //        .HasMaxLength(5)
        //        .HasColumnName("br_free_code_3");

        //    entity.Property(e => e.BrFreeCode4)
        //        .HasMaxLength(5)
        //        .HasColumnName("br_free_code_4");

        //    entity.Property(e => e.BrFreeCode5)
        //        .HasMaxLength(5)
        //        .HasColumnName("br_free_code_5");

        //    entity.Property(e => e.BrFreeCode6)
        //        .HasMaxLength(5)
        //        .HasColumnName("br_free_code_6");

        //    entity.Property(e => e.BrFreeCode7)
        //        .HasMaxLength(5)
        //        .HasColumnName("br_free_code_7");

        //    entity.Property(e => e.BrFreeCode8)
        //        .HasMaxLength(5)
        //        .HasColumnName("br_free_code_8");

        //    entity.Property(e => e.BrFreeCode9)
        //        .HasMaxLength(5)
        //        .HasColumnName("br_free_code_9");

        //    entity.Property(e => e.BrFreeCode10)
        //        .HasMaxLength(5)
        //        .HasColumnName("br_free_code_10");

        //    entity.Property(e => e.IsoCntryCode)
        //        .HasMaxLength(2)
        //        .HasColumnName("iso_cntry_code");

        //    entity.Property(e => e.CtsEnabledFlg)
        //        .HasMaxLength(1)
        //        .HasColumnName("cts_enabled_flg");

        //    entity.Property(e => e.PrefLangCode)
        //        .HasMaxLength(10)
        //        .HasColumnName("pref_lang_code");

        //    entity.Property(e => e.PrefLangBrName)
        //        .HasMaxLength(30)
        //        .HasColumnName("pref_lang_br_name");

        //    entity.Property(e => e.PrefLangBrShortName)
        //        .HasMaxLength(10)
        //        .HasColumnName("pref_lang_br_short_name");

        //    entity.Property(e => e.PrefLangBrAddr1)
        //        .HasMaxLength(45)
        //        .HasColumnName("pref_lang_br_addr_1");

        //    entity.Property(e => e.PrefLangBrAddr2)
        //        .HasMaxLength(45)
        //        .HasColumnName("pref_lang_br_addr_2");

        //    entity.Property(e => e.Bic)
        //        .HasMaxLength(12)
        //        .HasColumnName("bic");

        //    entity.Property(e => e.LocalBankCode)
        //        .HasMaxLength(34)
        //        .HasColumnName("local_bank_code");

        //    entity.Property(e => e.Alt1BrName)
        //        .HasMaxLength(30)
        //        .HasColumnName("alt1_br_name");

        //    entity.Property(e => e.Alt1BrShortName)
        //        .HasMaxLength(10)
        //        .HasColumnName("alt1_br_short_name");

        //    entity.Property(e => e.OtherEntityBankId)
        //        .HasMaxLength(8)
        //        .HasColumnName("other_entity_bank_id");

        //    entity.Property(e => e.PrefLangBrAddr3)
        //        .HasMaxLength(45)
        //        .HasColumnName("pref_lang_br_addr_3");

        //    entity.Property(e => e.SwiftBrName)
        //        .HasMaxLength(35)
        //        .HasColumnName("swift_br_name");

        //    entity.Property(e => e.SwiftBrAddr1)
        //        .HasMaxLength(35)
        //        .HasColumnName("swift_br_addr_1");

        //    entity.Property(e => e.SwiftBrAddr2)
        //        .HasMaxLength(35)
        //        .HasColumnName("swift_br_addr_2");

        //    entity.Property(e => e.SwiftBrAddr3)
        //        .HasMaxLength(35)
        //        .HasColumnName("swift_br_addr_3");

        //    entity.Property(e => e.Alt1LangBrAddr1)
        //        .HasMaxLength(45)
        //        .HasColumnName("alt1_lang_br_addr_1");

        //    entity.Property(e => e.Alt1LangBrAddr2)
        //        .HasMaxLength(45)
        //        .HasColumnName("alt1_lang_br_addr_2");

        //    entity.Property(e => e.Alt1LangBrAddr3)
        //        .HasMaxLength(45)
        //        .HasColumnName("alt1_lang_br_addr_3");

        //    entity.Property(e => e.FactorCode)
        //        .HasMaxLength(5)
        //        .HasColumnName("factor_code");

        //    entity.Property(e => e.ClgRepCode)
        //       .HasMaxLength(5)
        //       .HasColumnName("clg_rep_code");
        //});

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
