using System.Net.Http;
using System.Net.Http.Headers;
using Appointment_Scheduler.Services;


string authToken = "[Your API token goes here]";
SchedulingApiClient client = new SchedulingApiClient(authToken);
await client.PostStartScheduling();
AppointmentScheduler scheduler = new AppointmentScheduler(client);
await scheduler.LoadInitialScheduleAsync();
await scheduler.LoadAppointmentRequestsAsync();
await scheduler.ScheduleAppointmentsAsync();
await client.PostStopScheduling();






