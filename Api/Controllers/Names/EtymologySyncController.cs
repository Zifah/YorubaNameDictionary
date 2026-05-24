using Api.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YorubaOrganization.Core.Cache;

namespace Api.Controllers.Names
{
    [Route("api/v1/names/etymology-sync")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class EtymologySyncController(
        IBackgroundJobClientV2 backgroundJobClient,
        NameEtymologyDefinitionsSyncJob syncJob,
        ISimpleCache simpleCache) : ControllerBase
    {
        private readonly IBackgroundJobClientV2 _backgroundJobClient = backgroundJobClient;
        private readonly NameEtymologyDefinitionsSyncJob _syncJob = syncJob;
        private readonly ISimpleCache _simpleCache = simpleCache;

        [HttpPost("run")]
        public async Task<IActionResult> TriggerEtymologyDefinitionsSync()
        {
            var checkpointPage = await _simpleCache.GetAsync<int>(_syncJob.CheckpointPageKey);
            var jobId = _backgroundJobClient.Enqueue(() => _syncJob.ExecuteAsync());

            return Accepted(new
            {
                jobId,
                checkpointPage,
                nextPage = Math.Max(1, checkpointPage + 1)
            });
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetEtymologyDefinitionsSyncStatus()
        {
            var checkpointPage = await _simpleCache.GetAsync<int>(_syncJob.CheckpointPageKey);
            var lastProcessedPage = await _simpleCache.GetAsync<int>(_syncJob.LastProcessedPageKey);
            var lastRunId = await _simpleCache.GetAsync<string>(_syncJob.LastRunIdKey);
            var lastRunStatus = await _simpleCache.GetAsync<string>(_syncJob.LastRunStatusKey);
            var lastRunStartedAt = await _simpleCache.GetAsync<string>(_syncJob.LastRunStartedAtKey);
            var lastRunCompletedAt = await _simpleCache.GetAsync<string>(_syncJob.LastRunCompletedAtKey);
            var lastRunError = await _simpleCache.GetAsync<string>(_syncJob.LastRunErrorKey);

            return Ok(new
            {
                checkpointPage,
                lastProcessedPage,
                lastRunId,
                lastRunStatus,
                lastRunStartedAt,
                lastRunCompletedAt,
                lastRunError,
                nextPage = Math.Max(1, checkpointPage + 1)
            });
        }
    }
}
