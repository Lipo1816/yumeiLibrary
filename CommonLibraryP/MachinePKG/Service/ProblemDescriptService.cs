using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;

namespace CommonLibraryP.MachinePKG.Service
{
    public class ProblemDescriptService
    {
        private readonly MachineDBContext _context;

        public ProblemDescriptService(MachineDBContext context)
        {
            _context = context;
        }

        // 取得全部
        public async Task<List<ProblemDescript>> GetAllAsync()
        {
            return await _context.ProblemDescripts.ToListAsync();
        }

        // 依代碼取得
        public async Task<ProblemDescript?> GetByIdAsync(string defectCode)
        {
            return await _context.ProblemDescripts.FindAsync(defectCode);
        }

        // 新增或更新
        public async Task UpsertAsync(ProblemDescript entity)
        {
            var exist = await _context.ProblemDescripts.FindAsync(entity.不良代碼);
            if (exist == null)
                _context.ProblemDescripts.Add(entity);
            else
                _context.Entry(exist).CurrentValues.SetValues(entity);

            await _context.SaveChangesAsync();
        }

        // 刪除
        public async Task<bool> DeleteAsync(string defectCode)
        {
            var exist = await _context.ProblemDescripts.FindAsync(defectCode);
            if (exist == null) return false;
            _context.ProblemDescripts.Remove(exist);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
