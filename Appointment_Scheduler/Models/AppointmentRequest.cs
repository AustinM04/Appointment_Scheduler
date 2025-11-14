using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_Scheduler.Models
{
		/// <summary>
		/// DTO for the appointment request.
		/// </summary>
		public record class AppointmentRequest
		{
			/// <summary>
			/// Id of the appointment request.
			/// </summary>
			public int RequestId { get; set; }
			
			/// <summary>
			/// Id of the person requesting the appointment.
			/// </summary>
			public int PersonId { get; set; }
			
			/// <summary>
			/// Array of preferred days for the appointment.
			/// </summary>
			public DateTime[] PreferredDays { get; set; }
			
			/// <summary>
			/// Array of preferred doc ids for the appointment.
			/// </summary>
			public int[] PreferredDocs { get; set; }

			/// <summary>
			/// Bool to indicate if the patient is new.
			/// </summary>
			public bool IsNew { get; set; }

		}
}
