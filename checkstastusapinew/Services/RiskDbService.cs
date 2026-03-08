using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace checkstastusapinew.Services
{
    public class RiskDbService
    {
        private readonly string _connectionString;

        public RiskDbService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("RiskDatabase") 
                ?? throw new InvalidOperationException("Connection string 'RiskDatabase' not found.");
        }

        public async Task<List<Dictionary<string, object>>> GetTopUsersAsync(int top = 100)
        {
            var results = new List<Dictionary<string, object>>();
            var sql = "SELECT TOP (@top) * FROM [dbo].[Users];";

            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@top", SqlDbType.Int) { Value = top });

            await conn.OpenAsync();
            await using var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

            var cols = new List<string>();
            for (int i = 0; i < rdr.FieldCount; i++) cols.Add(rdr.GetName(i));

            while (await rdr.ReadAsync())
            {
                var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var c in cols)
                {
                    var idx = rdr.GetOrdinal(c);
                    var val = await rdr.IsDBNullAsync(idx) ? null : rdr.GetValue(idx);
                    row[c] = val;
                }
                results.Add(row);
            }

            return results;
        }

        public async Task<Dictionary<string, object>?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var sql = "SELECT TOP (1) * FROM [dbo].[Users] WHERE Username = @username;";

            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@username", SqlDbType.NVarChar, 200) { Value = username });

            await conn.OpenAsync();
            await using var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

            if (!await rdr.ReadAsync())
                return null;

            var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < rdr.FieldCount; i++)
            {
                var colName = rdr.GetName(i);
                var val = await rdr.IsDBNullAsync(i) ? null : rdr.GetValue(i);
                row[colName] = val;
            }

            return row;
        }
        public async Task<(List<Dictionary<string, object>> Data, int TotalCount)> GetRiskRegisterPageAsync(int page, int pageSize)
        {
            var results = new List<Dictionary<string, object>>();
            int totalCount = 0;
            var sql = @"SELECT COUNT(*) FROM [dbo].[RiskRegister];
                        SELECT * FROM [dbo].[RiskRegister]
                        ORDER BY Id
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(sql, conn);
            int offset = (page - 1) * pageSize;
            cmd.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
            cmd.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });

            await conn.OpenAsync();
            using (var rdr = await cmd.ExecuteReaderAsync())
            {
                // Read total count
                if (await rdr.ReadAsync())
                    totalCount = rdr.GetInt32(0);

                // Move to next result set (paged data)
                await rdr.NextResultAsync();

                var cols = new List<string>();
                for (int i = 0; i < rdr.FieldCount; i++) cols.Add(rdr.GetName(i));

                while (await rdr.ReadAsync())
                {
                    var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    foreach (var c in cols)
                    {
                        var idx = rdr.GetOrdinal(c);
                        var val = await rdr.IsDBNullAsync(idx) ? null : rdr.GetValue(idx);
                        row[c] = val;
                    }
                    results.Add(row);
                }
            }
            return (results, totalCount);
        }
        public async Task<int> InsertRiskAsync(
            int? riskRegisterId,
            string riskName,
            string riskDescription,
            string riskCategory,
            string directorate,
            string riskOwner,
            int? riskChampionId,
            int? frequency,
            string frequencyPeriod,
            int? impact,
            int? riskScore,
            string region,
            string district,
            string phoneNumber,
            string landmark,
            int? userId,
            string userName,
            string imageUrls
        )
        {
            var sql = @"INSERT INTO dbo.Risks
                (
                    RiskRegisterId,
                    RiskName,
                    RiskDescription,
                    RiskCategory,
                    Directorate,
                    RiskOwner,
                    RiskChampionId,
                    Frequency,
                    FrequencyPeriod,
                    Impact,
                    RiskScore,
                    RegionId,
                    DistrictId,
                    Region,
                    District,
                    PhoneNumber,
                    Landmark,
                    CreatedDate,
                    Status,
                    IsLocked,
                    UserId,
                    UserName,
                    ImageUrls
                )
                VALUES
                (
                    @RiskRegisterId,
                    @RiskName,
                    @RiskDescription,
                    @RiskCategory,
                    @Directorate,
                    @RiskOwner,
                    @RiskChampionId,
                    @Frequency,
                    @FrequencyPeriod,
                    @Impact,
                    @RiskScore,
                    '01',
                    '01',
                    @Region,
                    @District,
                    @PhoneNumber,
                    @Landmark,
                    GETDATE(),
                    'New Risk',
                    0,
                    @UserId,
                    @UserName,
                    @ImageUrls
                );
                SELECT SCOPE_IDENTITY();";

            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@RiskRegisterId", (object?)riskRegisterId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RiskName", (object?)riskName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RiskDescription", (object?)riskDescription ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RiskCategory", (object?)riskCategory ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Directorate", (object?)directorate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RiskOwner", (object?)riskOwner ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RiskChampionId", (object?)riskChampionId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Frequency", (object?)frequency ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FrequencyPeriod", (object?)frequencyPeriod ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Impact", (object?)impact ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RiskScore", (object?)riskScore ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Region", (object?)region ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@District", (object?)district ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhoneNumber", (object?)phoneNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Landmark", (object?)landmark ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserName", (object?)userName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ImageUrls", (object?)imageUrls ?? DBNull.Value);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}
