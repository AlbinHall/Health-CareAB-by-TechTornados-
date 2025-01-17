﻿using System;
using HealthCareABApi.Models;

namespace HealthCareABApi.Repositories
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment> GetByIdAsync(int id);
        Task CreateAsync(Appointment appointment);
        Task<bool> UpdateAsync(int id, Appointment appointment);
        Task DeleteAsync(int id);
        Task<IEnumerable<Appointment>> GetByUserIdAsync(int patientId);
        Task<Appointment> GetByPatientAndTimeAsync(int patientId, DateTime appointmentTime);
    }
}

