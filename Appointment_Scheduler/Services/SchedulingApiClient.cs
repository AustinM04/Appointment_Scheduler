using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Appointment_Scheduler.Models;

namespace Appointment_Scheduler.Services
{
		public class SchedulingApiClient(string accessToken)
		{
			private readonly HttpClient _client = new();

			public async Task<List<AppointmentInfo>?> GetScheduleAsync()
			{
				var schedule = await _client.GetFromJsonAsync<List<AppointmentInfo>>("https://scheduling.interviews.brevium.com/api/Scheduling/Schedule?token=" + accessToken);
				return schedule;
			}

			public async Task<AppointmentRequest?> GetAppointmentRequestAsync()
			{
				try
				{
					var result = await _client.GetFromJsonAsync<AppointmentRequest>(
						"https://scheduling.interviews.brevium.com/api/Scheduling/AppointmentRequest?token=" + accessToken);
					return result;
				}
				catch (JsonException exception)
				{
					return null;
				}
				
			}

			public async Task<HttpResponseMessage> PostScheduleAppointmentAsync(AppointmentInfoRequest request)
			{
				return await _client.PostAsJsonAsync("https://scheduling.interviews.brevium.com/api/Scheduling/ScheduleAppointment?token=" + accessToken, request);
			}

			public async Task<HttpResponseMessage> PostStartScheduling()
			{
				return await _client.PostAsync("https://scheduling.interviews.brevium.com/api/Scheduling/Start?token=" + accessToken, null);
			}

			public async Task<HttpResponseMessage> PostStopScheduling()
			{
				return await _client.PostAsync("https://scheduling.interviews.brevium.com/api/Scheduling/Stop?token=" + accessToken, null);
			}
		}
}
