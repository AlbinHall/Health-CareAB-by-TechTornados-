﻿using Microsoft.AspNetCore.Mvc;
using HealthCareABApi.Repositories;
using HealthCareABApi.Models;
using HealthCareABApi.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HealthCareABApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[Controller]")]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityRepository _availabilityRepository;

        public AvailabilityController(IAvailabilityRepository availabilityRepository)
        {
            _availabilityRepository = availabilityRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAvailability([FromBody] CreateAvailabilityDTO availabilityDto)
        {
            if (availabilityDto.StartTime >= availabilityDto.EndTime)
            {
                return BadRequest("StartTime must be earlier than EndTime.");
            }
            try
            {
                var caregiverClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                    ?? User.Claims.FirstOrDefault(c => c.Type == "sub");

                if (caregiverClaim == null)
                {
                    return Unauthorized("Did not find user, token may be outdated.");
                }

                if (!int.TryParse(caregiverClaim.Value, out var caregiverId))
                {
                    return BadRequest("Invalid caregiver ID format in token.");
                }

                var caregiver = await _availabilityRepository.GetCaregiverByIdAsync(caregiverId);
                if (caregiver == null)
                {
                    return NotFound("Caregiver not found.");
                }

                var availability = new Availability
                {
                    Caregiver = caregiver,
                    StartTime = availabilityDto.StartTime,
                    EndTime = availabilityDto.EndTime,
                    IsAvailable = true
                };

                await _availabilityRepository.CreateAsync(availability);
                return Ok(new { Message = "Availability created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });

            }
        }
    }
}