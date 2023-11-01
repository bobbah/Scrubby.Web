using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Models.Api;
using Scrubby.Web.Services.Interfaces;
using Scrubby.Web.Stockpile;

namespace Scrubby.Web.Controllers;

[Authorize(Roles = "Developer")]
public class AdminController : Controller
{
    private readonly IMaintenanceService _maintenance;
    private readonly IStockpileApi _stockpile;

    public AdminController(IMaintenanceService maintenance, IStockpileApi stockpile)
    {
        _maintenance = maintenance;
        _stockpile = stockpile;
    }

    [HttpGet("admin/reparse")]
    public IActionResult ReparseIndex()
    {
        return View();
    }

    [HttpPost("api/admin/reparse/ids")]
    public async Task<IActionResult> ReparseRoundById([FromBody] ReparseByIdsRequestModel model)
    {
        if (model?.Ids == null || !model.Ids.Any())
            return BadRequest("Invalid request: you did not provide any round id[s]");

        if (model.DeleteFiles)
        {
            // Get archives for each round
            var archives = new HashSet<Guid>();
            foreach (var round in model.Ids)
            {
                var candidateFiles = await _maintenance.GetObjectsForArchiveDiscovery(round);
                foreach (var file in candidateFiles)
                {
                    try
                    {
                        var metadata = await _stockpile.GetObjectMetadata(file);
                        if (metadata == null)
                            continue;
                        archives.Add(metadata.ParentArchive);
                    }
                    catch (RestEase.ApiException ex)
                    {
                        // Re-throw if not a 404; otherwise ignore as it just means the archive was already deleted
                        if (ex.StatusCode != HttpStatusCode.NotFound)
                            throw;
                    }
                }
            }

            // Delete archives
            foreach (var archive in archives)
            {
                await _stockpile.DeleteArchive(archive);
            }
        }

        await _maintenance.ReparseRoundsAsync(model.Ids, model.DeleteFiles);
        return Ok();
    }

    [HttpPost("api/admin/reparse/file")]
    public async Task<IActionResult> ReparseRoundByFile([FromBody] ReparseByFilenameRequestModel model)
    {
        if (string.IsNullOrWhiteSpace(model?.Filename))
            return BadRequest($"You provided an invalid filename: '{model?.Filename}'");

        await _maintenance.ReparseRoundsWithFileAsync(model.Filename);
        return Ok();
    }
}