using System.Globalization;
using Appointment_Scheduler.Models;

namespace Appointment_Scheduler.Services
{
		public class AppointmentScheduler(SchedulingApiClient schedulingApi)
		{
			private List<AppointmentRequest> _appointmentQueue = new();
			private List<AppointmentInfo> _currentSchedule = new();

			public async Task LoadInitialScheduleAsync()
			{
				var schedule = await schedulingApi.GetScheduleAsync();
				if (schedule is not null)
				{
					_currentSchedule.AddRange(schedule);
					Console.WriteLine(_currentSchedule);
				}
			}

			public async Task LoadAppointmentRequestsAsync()
			{
				while (true)
				{
					var request = await schedulingApi.GetAppointmentRequestAsync();
					if (request is null)
					{
						break;
					}
					_appointmentQueue.Add(request);
					Console.WriteLine(request);
				}
				Console.WriteLine("All requests read");
			}

			private static bool IsWeekday(DateTime dt)
			{
				return dt.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday;
			}

			private static bool IsInAllowedMonths(DateTime dt)
			{
				return dt.Year == 2021 && (dt.Month == 11 || dt.Month == 12);
			}

			private static bool IsWithinWorkingHours(DateTime dt)
			{
				return dt.Minute == 0 && dt.Hour >= 8 && dt.Hour <= 16;
			}

			private static bool IsValidHourForNewPatient(DateTime dt, bool isNew)
			{
				return !isNew || dt.Hour == 15 || dt.Hour == 16;
			}

			private bool IsDoctorFree(int doctorId, DateTime slot)
			{
				return _currentSchedule.Any(a =>
					a.DoctorId == doctorId &&
					DateTime.Parse(a.AppointmentTime) == slot);
			}

			private bool IsPatientSpacingValid(int personId, DateTime slot)
			{
				foreach (var a in _currentSchedule.Where(a => a.PersonId == personId))
				{
					var diff = (slot.Date - DateTime.Parse(a.AppointmentTime)).TotalDays;
					if (Math.Abs(diff) < 7)
					{
						return false;
					}
				}

				return true;
			}

			private IEnumerable<DateTime> CreateAvailableTimeslots(AppointmentRequest request)
			{
				var days = request.PreferredDays
					.Select(d => DateTime.Parse(d, null, System.Globalization.DateTimeStyles.RoundtripKind))
					.Where(IsInAllowedMonths)
					.OrderBy(d => d);
				List<DateTime> slots = new();

				foreach (var day in days)
				{
					if (IsWeekday(day))
					{
						var slot = new DateTime(day.Year, day.Month, day.Day, day.Hour, 0, 0, DateTimeKind.Utc);
						if (!IsWithinWorkingHours(slot))
							continue;

						if (!IsValidHourForNewPatient(slot, request.IsNew))
							continue;
						slots.Add(slot);
					}
				}

				return slots;
			}

			private AppointmentInfoRequest? BuildAppointment(AppointmentRequest request)
			{
				foreach (var slot in CreateAvailableTimeslots(request))
				{
					foreach (var doctorId in request.PreferredDocs)
					{
						if (!IsDoctorFree(doctorId, slot))
							continue;

						if (!IsPatientSpacingValid(request.PersonId, slot))
							continue;

						return new AppointmentInfoRequest
						{
							DoctorId = doctorId,
							PersonId = request.PersonId,
							AppointmentTime = slot.ToLongDateString(),
							IsNewPatientAppointment = request.IsNew,
							RequestId = request.RequestId
						};
					}
				}
				return null;
			}
			public async Task ScheduleAppointmentsAsync()
			{
				foreach (var request in _appointmentQueue)
				{
					var apptReq = BuildAppointment(request);
					if (apptReq is null)
					{
						Console.WriteLine($"Could not find slot for request {request.RequestId}");
						continue;
					}

					var response = await schedulingApi.PostScheduleAppointmentAsync(apptReq);
					if (!response.IsSuccessStatusCode)
					{
						Console.WriteLine($"Failed to schedule request {request.RequestId}: {response.StatusCode}");
						continue;
					}

					_currentSchedule.Add(new AppointmentInfo
					{
						DoctorId = apptReq.DoctorId,
						PersonId = apptReq.PersonId,
						AppointmentTime = apptReq.AppointmentTime,
						IsNewPatientAppointment = apptReq.IsNewPatientAppointment
					});

					Console.WriteLine(
						$"Scheduled request {request.RequestId} for person {request.PersonId} with doctor {apptReq.DoctorId} at {apptReq.AppointmentTime}");
				}
			}
		}
}
