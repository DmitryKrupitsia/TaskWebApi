using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskWebApi.Repositories;

namespace TaskWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public TaskController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet("task1/tvonly")]
        public async Task<IActionResult> GetTvOnly() =>
            Ok(await _taskRepository.GetCustomersWithOnlyTvNoDslAsync());

        [HttpGet("task1/dslonly")]
        public async Task<IActionResult> GetDslOnly() =>
            Ok(await _taskRepository.GetCustomersWithOnlyDslNoTvAsync());

        [HttpGet("task2/overlaps")]
        public async Task<IActionResult> GetOverlaps() =>
            Ok(await _taskRepository.GetOverlappingTvProductsAsync());

        [HttpGet("task3/linked")]
        public async Task<IActionResult> GetLinkedCustomers() =>
            Ok(await _taskRepository.GetLinkedCustomersAsync());
    }
}
