using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Entities;

public partial class CcemQatContext : DbContext
{
    public CcemQatContext()
    {
    }

    public CcemQatContext(DbContextOptions<CcemQatContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActionPlan> ActionPlans { get; set; }

    public virtual DbSet<Atc> Atcs { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BranchAccess> BranchAccesses { get; set; }

    public virtual DbSet<BranchReply> BranchReplies { get; set; }

    public virtual DbSet<Deviation> Deviations { get; set; }

    public virtual DbSet<DeviationCategoryLookup> DeviationCategoryLookups { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<ExceptionCode> ExceptionCodes { get; set; }

    public virtual DbSet<ExceptionCodeRev> ExceptionCodeRevs { get; set; }

    public virtual DbSet<ExceptionDeviationList> ExceptionDeviationLists { get; set; }

    public virtual DbSet<ExceptionItem> ExceptionItems { get; set; }

    public virtual DbSet<ExceptionItemRev> ExceptionItemRevs { get; set; }

    public virtual DbSet<ExceptionItemTableTest> ExceptionItemTableTests { get; set; }

    public virtual DbSet<ExceptionItemTableTest2> ExceptionItemTableTest2s { get; set; }

    public virtual DbSet<ExceptionItemTest> ExceptionItemTests { get; set; }

    public virtual DbSet<ExceptionItemTest3> ExceptionItemTest3s { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Misc> Miscs { get; set; }

    public virtual DbSet<MiscRev> MiscRevs { get; set; }

    public virtual DbSet<Monetary> Monetaries { get; set; }

    public virtual DbSet<MonetaryRev> MonetaryRevs { get; set; }

    public virtual DbSet<NonMonetary> NonMonetaries { get; set; }

    public virtual DbSet<NonMonetaryRev> NonMonetaryRevs { get; set; }

    public virtual DbSet<PermissionLookup> PermissionLookups { get; set; }

    public virtual DbSet<RefNoSequence> RefNoSequences { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<ReportContent> ReportContents { get; set; }

    public virtual DbSet<ReportContentsArchived> ReportContentsArchiveds { get; set; }

    public virtual DbSet<ReportsRev> ReportsRevs { get; set; }

    public virtual DbSet<RiskClassificationsLookup> RiskClassificationsLookups { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<SubRefNoSequence> SubRefNoSequences { get; set; }

    public virtual DbSet<Tltxmapping> Tltxmappings { get; set; }

    public virtual DbSet<User> Users { get; set; }

   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActionPlan>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ActionPlan1).HasColumnName("ActionPlan");
            entity.Property(e => e.ExceptionItemRefNo).HasMaxLength(450);
        });

        modelBuilder.Entity<Atc>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ATC");

            entity.Property(e => e.AtcCode).HasColumnName("ATC_Code");
            entity.Property(e => e.AtcTax)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("ATC_Tax");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.TaxCode)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<BranchAccess>(entity =>
        {
            entity.ToTable("BranchAccess");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.UsersLoginName).HasMaxLength(20);

            entity.HasOne(d => d.UsersLoginNameNavigation).WithMany(p => p.BranchAccesses).HasForeignKey(d => d.UsersLoginName);
        });

        modelBuilder.Entity<BranchReply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_BranchReply_1");

            entity.ToTable("BranchReply");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
        });

        modelBuilder.Entity<Deviation>(entity =>
        {
            entity.Property(e => e.Deviation1).HasColumnName("Deviation");
        });

        modelBuilder.Entity<DeviationCategoryLookup>(entity =>
        {
            entity.ToTable("DeviationCategoryLookup");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
        });

        modelBuilder.Entity<ExceptionCode>(entity =>
        {
            entity.ToTable("ExceptionCode");

            entity.Property(e => e.ExItemRefNo).HasMaxLength(450);

            entity.HasOne(d => d.ExItemRefNoNavigation).WithMany(p => p.ExceptionCodes).HasForeignKey(d => d.ExItemRefNo);
        });

        modelBuilder.Entity<ExceptionCodeRev>(entity =>
        {
            entity.Property(e => e.ActionTaken).HasMaxLength(25);
            entity.Property(e => e.ApprovedBy).HasMaxLength(20);
            entity.Property(e => e.Changes).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(20);
            entity.Property(e => e.Remarks)
                .HasMaxLength(300)
                .HasDefaultValue("");

            entity.HasOne(d => d.ExItem).WithMany(p => p.ExceptionCodeRevs).HasForeignKey(d => d.ExItemId);
        });

        modelBuilder.Entity<ExceptionDeviationList>(entity =>
        {
            entity.ToTable("ExceptionDeviationList");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.EntryDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LikelihoodDesc).HasColumnName("Likelihood_Desc");
            entity.Property(e => e.LikelihoodScore).HasColumnName("Likelihood_Score");
            entity.Property(e => e.MagnitudeDesc).HasColumnName("Magnitude_Desc");
            entity.Property(e => e.MagnitudeScore).HasColumnName("Magnitude_Score");
            entity.Property(e => e.RiskAssessmentScore).HasColumnName("RiskAssessment_Score");
        });

        modelBuilder.Entity<ExceptionItem>(entity =>
        {
            entity.HasKey(e => e.RefNo);

            entity.ToTable("ExceptionItem");

            entity.Property(e => e.Area).HasMaxLength(50);
            entity.Property(e => e.BranchCode).HasMaxLength(3);
            entity.Property(e => e.BranchName).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(25);
            entity.Property(e => e.Division).HasMaxLength(50);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .HasColumnName("EmployeeID");
            entity.Property(e => e.OtherPersonResponsible).HasMaxLength(100);
            entity.Property(e => e.PersonResponsible).HasMaxLength(100);
        });

        modelBuilder.Entity<ExceptionItemRev>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ActionTaken).HasMaxLength(25);
            entity.Property(e => e.ApprovedBy).HasMaxLength(20);
            entity.Property(e => e.Area).HasMaxLength(50);
            entity.Property(e => e.BranchCode).HasMaxLength(3);
            entity.Property(e => e.BranchName).HasMaxLength(50);
            entity.Property(e => e.Changes).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(25);
            entity.Property(e => e.Division).HasMaxLength(50);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .HasColumnName("EmployeeID");
            entity.Property(e => e.ModifiedBy).HasMaxLength(20);
            entity.Property(e => e.OtherPersonResponsible).HasMaxLength(100);
            entity.Property(e => e.PersonResponsible).HasMaxLength(100);

            entity.HasOne(d => d.MiscRevs).WithMany(p => p.ExceptionItemRevs).HasForeignKey(d => d.MiscRevsId);

            entity.HasOne(d => d.MonetaryRevs).WithMany(p => p.ExceptionItemRevs).HasForeignKey(d => d.MonetaryRevsId);

            entity.HasOne(d => d.NonMonetaryRevs).WithMany(p => p.ExceptionItemRevs).HasForeignKey(d => d.NonMonetaryRevsId);
        });

        modelBuilder.Entity<ExceptionItemTableTest>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ExceptionItemTableTest$");

            entity.Property(e => e.ApprovalRemarks).HasMaxLength(255);
            entity.Property(e => e.Area).HasMaxLength(255);
            entity.Property(e => e.BranchName).HasMaxLength(255);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DeviationApprovedBy).HasMaxLength(255);
            entity.Property(e => e.DeviationApprover).HasMaxLength(255);
            entity.Property(e => e.Division).HasMaxLength(255);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.Id).HasMaxLength(255);
            entity.Property(e => e.OtherPersonResponsible).HasMaxLength(255);
            entity.Property(e => e.OtherRemarks).HasMaxLength(255);
            entity.Property(e => e.PersonResponsible).HasMaxLength(255);
            entity.Property(e => e.RefNo).HasMaxLength(255);
            entity.Property(e => e.Remarks).HasMaxLength(255);
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ExceptionItemTableTest2>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ExceptionItemTableTest2");

            entity.Property(e => e.ApprovalRemarks).HasMaxLength(255);
            entity.Property(e => e.Area).HasMaxLength(255);
            entity.Property(e => e.BranchName).HasMaxLength(255);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DeviationApprovedBy).HasMaxLength(255);
            entity.Property(e => e.DeviationApprover).HasMaxLength(255);
            entity.Property(e => e.Division).HasMaxLength(255);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.Id).HasMaxLength(255);
            entity.Property(e => e.OtherPersonResponsible).HasMaxLength(255);
            entity.Property(e => e.OtherRemarks).HasMaxLength(255);
            entity.Property(e => e.PersonResponsible).HasMaxLength(255);
            entity.Property(e => e.RefNo).HasMaxLength(255);
            entity.Property(e => e.Remarks).HasMaxLength(255);
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ExceptionItemTest>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ExceptionItemTest");

            entity.Property(e => e.Area).HasMaxLength(50);
            entity.Property(e => e.BranchCode).HasMaxLength(3);
            entity.Property(e => e.BranchName).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(25);
            entity.Property(e => e.Division).HasMaxLength(50);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .HasColumnName("EmployeeID");
            entity.Property(e => e.OtherPersonResponsible).HasMaxLength(100);
            entity.Property(e => e.PersonResponsible).HasMaxLength(100);
            entity.Property(e => e.RefNo).HasMaxLength(450);
        });

        modelBuilder.Entity<ExceptionItemTest3>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ExceptionItemTest3");

            entity.Property(e => e.ApprovalRemarks).HasMaxLength(255);
            entity.Property(e => e.Area).HasMaxLength(255);
            entity.Property(e => e.BranchName).HasMaxLength(255);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DeviationApprovedBy).HasMaxLength(255);
            entity.Property(e => e.DeviationApprover).HasMaxLength(255);
            entity.Property(e => e.Division).HasMaxLength(255);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.OtherPersonResponsible).HasMaxLength(255);
            entity.Property(e => e.OtherRemarks).HasMaxLength(255);
            entity.Property(e => e.PersonResponsible).HasMaxLength(255);
            entity.Property(e => e.RefNo).HasMaxLength(255);
            entity.Property(e => e.Remarks).HasMaxLength(255);
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Misc>(entity =>
        {
            entity.ToTable("Misc");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BankCertNo).HasMaxLength(25);
            entity.Property(e => e.CardNo).HasMaxLength(25);
            entity.Property(e => e.CheckNo).HasMaxLength(20);
            entity.Property(e => e.Dpafno)
                .HasMaxLength(20)
                .HasColumnName("DPAFNo");
            entity.Property(e => e.ExceptionId).HasColumnName("ExceptionID");
            entity.Property(e => e.GlslaccountName)
                .HasMaxLength(255)
                .HasColumnName("GLSLAccountName");
            entity.Property(e => e.GlslaccountNo)
                .HasMaxLength(50)
                .HasColumnName("GLSLAccountNo");
            entity.Property(e => e.RefNo).HasMaxLength(450);

            entity.HasOne(d => d.RefNoNavigation).WithMany(p => p.Miscs).HasForeignKey(d => d.RefNo);
        });

        modelBuilder.Entity<MiscRev>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BankCertNo).HasMaxLength(25);
            entity.Property(e => e.CardNo).HasMaxLength(25);
            entity.Property(e => e.CheckNo).HasMaxLength(20);
            entity.Property(e => e.Dpafno)
                .HasMaxLength(20)
                .HasColumnName("DPAFNo");
            entity.Property(e => e.ExceptionId).HasColumnName("ExceptionID");
            entity.Property(e => e.GlslaccountName)
                .HasMaxLength(255)
                .HasColumnName("GLSLAccountName");
            entity.Property(e => e.GlslaccountNo)
                .HasMaxLength(50)
                .HasColumnName("GLSLAccountNo");
        });

        modelBuilder.Entity<Monetary>(entity =>
        {
            entity.ToTable("Monetary");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BdstellerId)
                .HasMaxLength(4)
                .HasColumnName("BDSTellerID");
            entity.Property(e => e.ExceptionId).HasColumnName("ExceptionID");
            entity.Property(e => e.RefNo).HasMaxLength(450);

            entity.HasOne(d => d.RefNoNavigation).WithMany(p => p.Monetaries).HasForeignKey(d => d.RefNo);
        });

        modelBuilder.Entity<MonetaryRev>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BdstellerId)
                .HasMaxLength(4)
                .HasColumnName("BDSTellerID");
            entity.Property(e => e.ExceptionId).HasColumnName("ExceptionID");
        });

        modelBuilder.Entity<NonMonetary>(entity =>
        {
            entity.ToTable("NonMonetary");

            entity.Property(e => e.Cifnumber).HasColumnName("CIFNumber");
            entity.Property(e => e.ExceptionId).HasColumnName("ExceptionID");
            entity.Property(e => e.RefNo).HasMaxLength(450);

            entity.HasOne(d => d.RefNoNavigation).WithMany(p => p.NonMonetaries).HasForeignKey(d => d.RefNo);
        });

        modelBuilder.Entity<NonMonetaryRev>(entity =>
        {
            entity.Property(e => e.Cifnumber).HasColumnName("CIFNumber");
            entity.Property(e => e.ExceptionId).HasColumnName("ExceptionID");
        });

        modelBuilder.Entity<RefNoSequence>(entity =>
        {
            entity.ToTable("RefNoSequence");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.Property(e => e.Ccrecipients).HasColumnName("CCRecipients");
        });

        modelBuilder.Entity<ReportContent>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Aging).HasMaxLength(10);
            entity.Property(e => e.AgingCategory).HasMaxLength(20);
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Area).HasMaxLength(30);
            entity.Property(e => e.BranchCode).HasMaxLength(3);
            entity.Property(e => e.Division).HasMaxLength(30);
            entity.Property(e => e.EncodedBy).HasMaxLength(50);
            entity.Property(e => e.ExceptionNo).HasMaxLength(20);
            entity.Property(e => e.Process).HasMaxLength(50);
            entity.Property(e => e.RootCause).HasMaxLength(50);
            entity.Property(e => e.TransactionDate).HasMaxLength(20);

            entity.HasOne(d => d.Report).WithMany(p => p.ReportContents)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReportContents_Reports");
        });

        modelBuilder.Entity<ReportContentsArchived>(entity =>
        {
            entity.ToTable("ReportContents_Archived");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BranchCode).HasMaxLength(50);
            entity.Property(e => e.ExceptionNo).HasMaxLength(50);
        });

        modelBuilder.Entity<ReportsRev>(entity =>
        {
            entity.Property(e => e.ActionTaken).HasMaxLength(25);
            entity.Property(e => e.ApprovedBy).HasMaxLength(20);
            entity.Property(e => e.Ccrecipients).HasColumnName("CCRecipients");
            entity.Property(e => e.Changes).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(20);
        });

        modelBuilder.Entity<RiskClassificationsLookup>(entity =>
        {
            entity.ToTable("RiskClassificationsLookup");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<SubRefNoSequence>(entity =>
        {
            entity.ToTable("SubRefNoSequence");

            entity.Property(e => e.Ern).HasColumnName("ERN");
        });

        modelBuilder.Entity<Tltxmapping>(entity =>
        {
            entity.ToTable("TLTXMapping");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccType).HasMaxLength(255);
            entity.Property(e => e.AmountComputedLoc).HasMaxLength(255);
            entity.Property(e => e.AppType).HasMaxLength(255);
            entity.Property(e => e.DebitCredit).HasMaxLength(255);
            entity.Property(e => e.GlaccountDesc)
                .HasMaxLength(255)
                .HasColumnName("GLAccountDesc");
            entity.Property(e => e.GlaccountNo).HasColumnName("GLAccountNo");
            entity.Property(e => e.HostTranDesc).HasMaxLength(255);
            entity.Property(e => e.TltxrecordDesc)
                .HasMaxLength(255)
                .HasColumnName("TLTXRecordDesc");
            entity.Property(e => e.TltxrecordNo).HasColumnName("TLTXRecordNo");
            entity.Property(e => e.TltxtranCode).HasColumnName("TLTXTranCode");
            entity.Property(e => e.TltxtranDesc)
                .HasMaxLength(255)
                .HasColumnName("TLTXTranDesc");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.LoginName);

            entity.Property(e => e.LoginName).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .HasColumnName("EmployeeID");
            entity.Property(e => e.FirstName).HasMaxLength(30);
            entity.Property(e => e.Ipaddress).HasColumnName("IPAddress");
            entity.Property(e => e.LastName).HasMaxLength(20);
            entity.Property(e => e.MiddleName).HasMaxLength(20);
            entity.Property(e => e.TempIpaddress).HasColumnName("TempIPAddress");

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasForeignKey(d => d.RoleId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
