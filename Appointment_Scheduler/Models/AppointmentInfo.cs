using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_Scheduler.Models
{
		/// <summary>
		/// DTO for the appointment information.
		/// </summary>
		public class AppointmentInfo
		{
			/// <summary>
			/// Id of the doctor
			/// </summary>
			private int DoctorId { get; set; }

			/// <summary>
			/// Id of the person who is booking the appointment
			/// </summary>
			private int PersonId { get; set; }

			/// <summary>
			/// Time of the appointment
			/// </summary>
			public DateTime AppointmentTime { get; set; }

			/// <summary>
			/// Boolean indicating if the appointment is for a new patient
			/// </summary>
			public bool IsNewPatientAppointment { get; set; }

		}

}
