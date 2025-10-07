using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public class MachineDBContext : DbContext
    {
        public MachineDBContext(DbContextOptions<MachineDBContext> options) : base(options)
        {

        }


        public DbSet<Personnal> Personnal { get; set; }




        public virtual DbSet<ModbusSlaveConfig> ModbusSlaveConfigs { get; set; }

        public virtual DbSet<Machine> Machines { get; set; }
        public virtual DbSet<MachineStatusLog> MachineStatusLogs { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<ModbusTCPTag> ModbusTCPTags { get; set; }

        public virtual DbSet<TagCategory> TagCategories { get; set; }

        public virtual DbSet<EquipmentSpec> EquipmentSpecs { get; set; }

        public DbSet<WorkorderCheck> WorkorderChecks { get; set; }

        public DbSet<Workorder> Workorders { get; set; }
        public DbSet<WorkorderList> WorkorderLists { get; set; }
        public DbSet<InspectionRecord> InspectionRecords { get; set; }
        public DbSet<InspectionList> InspectionLists { get; set; }

        public DbSet<Inspection> Inspections { get; set; }

        public DbSet<InspectionReportTime> InspectionReportTimes { get; set; }

        public DbSet<ProblemDescript> ProblemDescripts { get; set; }

        public DbSet<ReportWorkOrder> ReportWorkOrders { get; set; }
        public DbSet<EquipmentSpecLimit> EquipmentSpecLimits { get; set; }
        public DbSet<InspecWOList> InspecWOLists { get; set; }

        public DbSet<EquiManufacturer_Information> EquiManufacturer_Informations { get; set; }

        public DbSet<temprature_Hu_log> temprature_Hu_logs { get; set; }

        public DbSet<temprature_Hu> temprature_Hus { get; set; }

        public DbSet<TagRecordData> TagRecordDatas { get; set; }
        public DbSet<Inspection_WoItem> Inspection_WoItem { get; set; }
        public DbSet<BreakTimeSchedule> BreakTimeSchedules { get; set; }

        public DbSet<WorkOrderPersonRecord> WorkOrderPersonRecords { get; set; }
        public DbSet<Data_Permission> Data_Permissions { get; set; }

        public DbSet<CarbonGeneratorParameter> CarbonGeneratorParameters { get; set; }
        public DbSet<EmailSentSetting> EmailSentSettings { get; set; }

        public DbSet<Data_Person_Permissions> DataPersonPermissions { get; set; }
        //public virtual DbSet<Condition> Conditions { get; set; }

        //public virtual DbSet<ConditionNode> ConditionNodes { get; set; }

        //public virtual DbSet<ConditionAction> ConditionActions { get; set; }


        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            // 統一設定所有 decimal / decimal? 的精度與小數位
            builder.Properties<decimal>().HavePrecision(18, 6);
            builder.Properties<decimal?>().HavePrecision(18, 6);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EquiManufacturer_Information>(entity =>
            {
                entity.ToTable("EquiManufacturer_Information");
                entity.HasKey(e => e.Id);
            });
            modelBuilder.Entity<Inspection_WoItem>(entity =>
            {
                entity.HasKey(e => new { e.點檢單號, e.點檢項目 });
                entity.Property(e => e.點檢單號).HasMaxLength(50).IsRequired();
                entity.Property(e => e.點檢項目).HasMaxLength(100).IsRequired();
                entity.Property(e => e.點檢時間).IsRequired();
                entity.Property(e => e.錯誤項目).HasMaxLength(50);
                entity.Property(e => e.點檢內容).HasMaxLength(200);
                entity.Property(e => e.錯誤代碼).HasMaxLength(50);
                entity.Property(e => e.分類).HasMaxLength(50);
                entity.Property(e => e.備註).HasMaxLength(200);
                entity.Property(e => e.責任單位).HasMaxLength(100);
                entity.Property(e => e.結果).HasMaxLength(50);
            });
            modelBuilder.Entity<ModbusSlaveConfig>(entity =>
            {

                entity.ToTable("ModbusSlaveConfigs");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Ip).HasColumnName("Ip");
                entity.Property(e => e.Port).HasColumnName("Port");
                entity.Property(e => e.Station).HasColumnName("Station");
            });

            modelBuilder.Entity<Machine>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.UseTpcMappingStrategy();
                entity.ToTable("Machine");

                entity.HasIndex(e => e.Name).IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.Ip)
                    .HasMaxLength(50)
                    .HasColumnName("IP");

                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.TagCategoryId).HasColumnName("TagCategoryID");
                //entity.Property(e => e.LogicStatusCategoryId).HasColumnName("LogicStatusCategoryID");


                entity.HasOne(d => d.TagCategory).WithMany(p => p.Machines)
                    .HasForeignKey(d => d.TagCategoryId);

                entity.Property(e => e.Enabled).HasColumnName("Enabled");
                entity.Property(e => e.UpdateDelay).HasColumnName("UpdateDelay");
                entity.Property(e => e.MaxRetryCount).HasColumnName("MaxRetryCount");
                entity.Property(e => e.RecordStatusChanged).HasColumnName("RecordStatusChanged");

            });

            modelBuilder.Entity<MachineStatusLog>(entity =>
            {

                entity.ToTable("MachineStatusLogs");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.MachineID).HasColumnName("MachineID");
                entity.Property(e => e.Status).HasColumnName("Status");
                entity.Property(e => e.LogTime).HasColumnName("LogTime");
            });

            modelBuilder.Entity<TagCategory>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TagCategory");

                entity.HasIndex(e => e.Name).IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.Id);

                //entity.ToTable("Tag");
                entity.UseTpcMappingStrategy();

                entity.HasIndex(e => e.Name).IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("ID");
                

                entity.HasOne(d => d.Category).WithMany(p => p.Tags)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientCascade);
            });

            modelBuilder.Entity<ModbusTCPTag>(entity =>
            {

                //entity.ToTable("ConditionLogicNodes");

            });

            modelBuilder.Entity<WorkOrderPersonRecord>().HasKey(x => new { x.姓名, x.工單, x.時間 });


            modelBuilder.Entity<EquipmentSpec>(entity =>
            {
                entity.HasKey(e =>  e.Id );
                entity.ToTable("EquipmentSpecs");
                entity.Property(e => e.項目).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.機台名稱).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.機種代碼).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.機台編號).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.線別編號).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.資訊項目).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.機台項目說明).HasMaxLength(200).IsRequired(false);
                entity.Property(e => e.機台項目代碼).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.規格型號).HasMaxLength(200).IsRequired(false);
                entity.Property(e => e.說明1).HasMaxLength(200).IsRequired(false);
                entity.Property(e => e.PLC讀值型態).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.PLC_XY位址).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.PLC讀值位址ModbusAdd).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.條件或格式).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.電控制箱編號).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.電控制箱IP).HasMaxLength(100).IsRequired(false);
            });

            modelBuilder.Entity<Personnal>(entity =>
            {
                entity.HasKey(e => e.人員ID);
                entity.ToTable("Personnal");
                entity.Property(e => e.人員ID)
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(e => e.部門ID)
                    .HasMaxLength(50)
                    .IsRequired(false);
                entity.Property(e => e.部門名稱)
                    .HasMaxLength(100)
                    .IsRequired(false);
                entity.Property(e => e.生產組名)
                    .HasMaxLength(100)
                    .IsRequired(false);
                entity.Property(e => e.人員姓名)
                    .HasMaxLength(50)
                    .IsRequired(false);
                entity.Property(e => e.職級代號)
                    .HasMaxLength(50)
                    .IsRequired(false);
                entity.Property(e => e.職級名稱)
                    .HasMaxLength(50)
                    .IsRequired(false);
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsRequired(false);
                entity.Property(e => e.權限)
                    .HasMaxLength(100)
                    .IsRequired(false);
                entity.Property(e => e.權限頁面)
                    .HasMaxLength(200)
                    .IsRequired(false);
            });


            modelBuilder.Entity<Workorder>(entity =>
            {
                entity.HasKey(e => e.工單號);
                entity.ToTable("Workorders");

                // 指定字串欄位長度（可依實際需求調整）
                entity.Property(e => e.工單號).HasMaxLength(50).IsRequired();
                entity.Property(e => e.料號).HasMaxLength(50).IsRequired();
                entity.Property(e => e.品名).HasMaxLength(100).IsRequired();
                entity.Property(e => e.訂單號).HasMaxLength(50).IsRequired(false);
                entity.Property(e => e.生產組別).HasMaxLength(50).IsRequired();
                entity.Property(e => e.生產線別).HasMaxLength(50).IsRequired();
                entity.Property(e => e.客戶編號).HasMaxLength(50).IsRequired(false);
                entity.Property(e => e.製程程式).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.發料儲位).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.物料採購單1).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.物料採購單2).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.物料採購單3).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.工單計算方式).HasMaxLength(50).IsRequired(false);

                // 指定 decimal 欄位精度
                entity.Property(e => e.工單發料量).HasPrecision(18, 4).IsRequired();
                entity.Property(e => e.分盒總重量).HasPrecision(18, 4).IsRequired();
                entity.Property(e => e.標準工時).HasPrecision(18, 4).IsRequired();

                // 其他 int/DateTime 欄位可依需求加上 .IsRequired()
                entity.Property(e => e.排產日).IsRequired();
                entity.Property(e => e.出貨日).IsRequired();
                entity.Property(e => e.分盒數).IsRequired();
            });
            modelBuilder.Entity<WorkorderCheck>(entity =>
            {
                entity.Property(e => e.工單發料量).HasPrecision(18, 4);
                entity.Property(e => e.分盒總重量).HasPrecision(18, 4);
                entity.Property(e => e.標準工時).HasPrecision(18, 4);
            });


            modelBuilder.Entity<Inspection>(entity =>
            {
                entity.HasKey(e => new { e.機台編號, e.項目 });
                entity.Property(e => e.頻率).IsRequired();
                // 其他欄位如需長度限制可加 .HasMaxLength(x)
            });
            modelBuilder.Entity<BreakTimeSchedule>()
    .HasKey(b => new { b.LineName, b.WeekDay, b.PeriodNo, b.ModifyTime });

        }
    }
}
