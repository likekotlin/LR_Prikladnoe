﻿using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using lrs.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace lrs.Controllers
{
    [Route("api/partworlds")]
    [ApiController]
    public class PartWorldsController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public PartWorldsController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet("{id}", Name = "GetPartWorld")]
        public async Task<IActionResult> GetPartWorldAsync(Guid id)
        {
            var partWorld = await _repository.PartWorld.GetPartWorldAsync(id, false);
            if (partWorld == null)
            {
                _logger.LogInfo($"PartWorld with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var partWorldDto = _mapper.Map<PartWorldDto>(partWorld);
            return Ok(partWorldDto);
        }
        [HttpGet]
        public async Task<IActionResult> GetPartWorldsAsync()
        {
            var partWorlds = await _repository.PartWorld.GetAllPartWorldsAsync(false);
            var partWorldsDto = _mapper.Map<IEnumerable<PartWorldDto>>(partWorlds);
            return Ok(partWorldsDto);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartWorldAsync(Guid id)
        {
            var partWorld = await _repository.PartWorld.GetPartWorldAsync(id, false);
            if (partWorld == null)
            {
                _logger.LogInfo($"PartWorld with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _repository.PartWorld.DeletePartWorld(partWorld);
            await _repository.SaveAsync();
            return NoContent();
        }
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreatePartWorldAsync([FromBody] PartWorldCreateDto partWorld)
        {
            var partWorldEntity = _mapper.Map<PartWorld>(partWorld);
            _repository.PartWorld.CreatePartWorld(partWorldEntity);
            await _repository.SaveAsync();
            var partWorldReturn = _mapper.Map<PartWorldDto>(partWorldEntity);
            return CreatedAtRoute("GetPartWorld", new
            {
                partWorldReturn.Id
            }, partWorldReturn);
        }
        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidatePartWorldExistsAttribute))]
        public async Task<IActionResult> UpdatePartWorldAsync(Guid id, [FromBody] PartWorldUpdateDto partWorld)
        {
            var partWorldEntity = HttpContext.Items["partWorld"] as PartWorld;
            _mapper.Map(partWorld, partWorldEntity);
            _repository.SaveAsync();
            return NoContent();
        }
    }
}
