﻿using Microsoft.Data.SqlClient;

namespace SpiritualNetwork.API.Services
{
	public class ConfigurationRepository
	{
		private readonly string _connectionString;

		public ConfigurationRepository(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task<string?> GetConfigurationValueAsync(string key)
		{
			using var connection = new SqlConnection(_connectionString);
			await connection.OpenAsync();

			var query = "SELECT Value FROM GlobalSetting WHERE KeyName = @Key";
			using var command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@Key", key);

			var result = await command.ExecuteScalarAsync();
			return result as string;
		}
	}

}