using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class RecordService
{
    private readonly RecordDbContext _context;

    public RecordService(RecordDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateOrUpdateRecordAsync(Record record)
    {
        var existingRecord = await _context.Records
            .Where(r => r.ImageName == record.ImageName && r.DifficultyLevel == record.DifficultyLevel)
            .FirstOrDefaultAsync();

        bool isNewRecord = false;

        if (existingRecord == null || existingRecord.BestTime > record.BestTime)
        {
            if (existingRecord != null)
            {
                _context.Records.Remove(existingRecord);
            }

            _context.Records.Add(record);
            await _context.SaveChangesAsync();

            isNewRecord = true;
        }
        return isNewRecord;
    }


    public async Task DeleteAllRecordsAsync()
    {
        foreach (var record in _context.Records)
        {
            _context.Records.Remove(record);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Record> GetRecordsAsync(string imageName, string difficultyLevel)
    {
        return await _context.Records
            .Where(r => r.ImageName == imageName && r.DifficultyLevel == difficultyLevel)
            .FirstOrDefaultAsync();
    }
}
